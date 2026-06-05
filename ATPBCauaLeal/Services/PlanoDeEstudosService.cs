using ATPBCauaLeal.Data;
using ATPBCauaLeal.Models;
using Microsoft.EntityFrameworkCore;

namespace ATPBCauaLeal.Services;

public class PlanoDeEstudosService
{
    private readonly ApplicationDbContext _context;

    public PlanoDeEstudosService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> PossuiPlanoAsync(string alunoId)
    {
        return await _context.PlanosDeEstudos
            .AnyAsync(plano => plano.AlunoId == alunoId);
    }

    public async Task<SelecaoPlanoResultado> ObterSelecaoAsync(ApplicationUser aluno)
    {
        if (!aluno.CursoId.HasValue)
        {
            return new SelecaoPlanoResultado(null, new(), new());
        }

        var disciplinasFaltantes = await _context.PlanosDeEstudos
            .Where(plano => plano.AlunoId == aluno.Id)
            .SelectMany(plano => plano.Disciplinas)
            .Select(item => item.DisciplinaId)
            .ToListAsync();

        var existePlano = disciplinasFaltantes.Count > 0 || await PossuiPlanoAsync(aluno.Id);

        var disciplinas = await _context.Disciplinas
            .Where(disciplina => disciplina.CursoId == aluno.CursoId.Value)
            .OrderBy(disciplina => disciplina.Nome)
            .Select(disciplina => new DisciplinaOpcao(
                disciplina.Id,
                disciplina.Codigo,
                disciplina.Nome,
                disciplina.CargaHoraria,
                disciplina.Obrigatoria,
                existePlano && !disciplinasFaltantes.Contains(disciplina.Id)))
            .ToListAsync();

        return new SelecaoPlanoResultado(
            aluno.Curso?.Nome,
            disciplinas.Where(disciplina => disciplina.Obrigatoria).ToList(),
            disciplinas.Where(disciplina => !disciplina.Obrigatoria).ToList());
    }

    public async Task SalvarPlanoAsync(ApplicationUser aluno, List<int> disciplinasConcluidas)
    {
        if (!aluno.CursoId.HasValue)
        {
            return;
        }

        var disciplinasDoCurso = await _context.Disciplinas
            .Where(disciplina => disciplina.CursoId == aluno.CursoId.Value)
            .ToListAsync();

        var idsDisciplinasDoCurso = disciplinasDoCurso
            .Select(disciplina => disciplina.Id)
            .ToList();

        var concluidasValidas = disciplinasConcluidas
            .Where(idsDisciplinasDoCurso.Contains)
            .Distinct()
            .ToList();

        var disciplinasFaltantes = disciplinasDoCurso
            .Where(disciplina => !concluidasValidas.Contains(disciplina.Id))
            .Select(disciplina => disciplina.Id)
            .ToList();

        var plano = await _context.PlanosDeEstudos
            .Include(plano => plano.Disciplinas)
            .FirstOrDefaultAsync(plano => plano.AlunoId == aluno.Id);

        if (plano is null)
        {
            plano = new PlanoDeEstudos
            {
                AlunoId = aluno.Id,
                CursoId = aluno.CursoId.Value,
                CriadoEm = DateTime.Now
            };

            _context.PlanosDeEstudos.Add(plano);
        }
        else
        {
            plano.CursoId = aluno.CursoId.Value;
            plano.ConcluidoEm = null;
            _context.PlanoDeEstudosDisciplinas.RemoveRange(plano.Disciplinas);
        }

        plano.Disciplinas = disciplinasFaltantes
            .Select(id => new PlanoDeEstudosDisciplina { DisciplinaId = id })
            .ToList();

        await _context.SaveChangesAsync();
    }

    public async Task<PlanoMontadoResultado?> ObterPlanoMontadoAsync(string alunoId)
    {
        var plano = await _context.PlanosDeEstudos
            .Include(plano => plano.Curso)
            .Include(plano => plano.Disciplinas)
            .ThenInclude(item => item.Disciplina)
            .FirstOrDefaultAsync(plano => plano.AlunoId == alunoId);

        if (plano is null)
        {
            return null;
        }

        var disciplinasFaltantes = plano.Disciplinas
            .Where(item => item.Disciplina is not null)
            .Select(item => item.Disciplina!)
            .OrderBy(disciplina => disciplina.Nome)
            .ToList();

        return new PlanoMontadoResultado(
            plano.Curso?.Nome,
            disciplinasFaltantes
                .Where(disciplina => disciplina.Obrigatoria)
                .Select(CriarDisciplinaPlano)
                .ToList(),
            disciplinasFaltantes
                .Where(disciplina => !disciplina.Obrigatoria)
                .Select(CriarDisciplinaPlano)
                .ToList(),
            disciplinasFaltantes.Sum(disciplina => disciplina.CargaHoraria));
    }

    public async Task<bool> DisciplinaPertenceAoPlanoAsync(string alunoId, int disciplinaId)
    {
        return await _context.PlanosDeEstudos
            .Where(plano => plano.AlunoId == alunoId)
            .SelectMany(plano => plano.Disciplinas)
            .AnyAsync(item => item.DisciplinaId == disciplinaId);
    }

    private static DisciplinaPlano CriarDisciplinaPlano(Disciplina disciplina)
    {
        return new DisciplinaPlano(
            disciplina.Id,
            disciplina.Codigo,
            disciplina.Nome,
            disciplina.CargaHoraria);
    }
}

public record SelecaoPlanoResultado(
    string? NomeCurso,
    List<DisciplinaOpcao> DisciplinasObrigatorias,
    List<DisciplinaOpcao> DisciplinasOptativas);

public record PlanoMontadoResultado(
    string? NomeCurso,
    List<DisciplinaPlano> ObrigatoriasFaltantes,
    List<DisciplinaPlano> OptativasFaltantes,
    int CargaHorariaFaltante);

public record DisciplinaOpcao(
    int Id,
    string Codigo,
    string Nome,
    int CargaHoraria,
    bool Obrigatoria,
    bool Concluida);

public record DisciplinaPlano(int Id, string Codigo, string Nome, int CargaHoraria);
