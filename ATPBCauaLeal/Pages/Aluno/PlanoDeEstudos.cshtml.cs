using ATPBCauaLeal.Data;
using ATPBCauaLeal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ATPBCauaLeal.Pages.Aluno;

[Authorize(Roles = "Aluno")]
public class PlanoDeEstudosModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public PlanoDeEstudosModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [BindProperty]
    public List<int> DisciplinasConcluidas { get; set; } = new();

    public string? NomeCurso { get; set; }

    public List<DisciplinaOpcao> DisciplinasObrigatorias { get; set; } = new();

    public List<DisciplinaOpcao> DisciplinasOptativas { get; set; } = new();

    [TempData]
    public string? MensagemSucesso { get; set; }

    [TempData]
    public string? MensagemErro { get; set; }

    public async Task<IActionResult> OnGetAsync(bool editar = false)
    {
        var aluno = await ObterAlunoAtualAsync();

        if (aluno is null)
        {
            return RedirectToPage("/Index");
        }

        var possuiPlano = await _context.PlanosDeEstudos
            .AnyAsync(plano => plano.AlunoId == aluno.Id);

        if (possuiPlano && !editar)
        {
            return RedirectToPage("/Aluno/PlanoMontado");
        }

        await CarregarDadosAsync(aluno);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var aluno = await ObterAlunoAtualAsync();

        if (aluno is null)
        {
            return RedirectToPage("/Index");
        }

        if (!aluno.CursoId.HasValue)
        {
            MensagemErro = "Seu curso ainda nao foi definido pelo administrador.";
            return RedirectToPage();
        }

        var disciplinasDoCurso = await _context.Disciplinas
            .Where(disciplina => disciplina.CursoId == aluno.CursoId.Value)
            .ToListAsync();

        var idsDisciplinasDoCurso = disciplinasDoCurso.Select(disciplina => disciplina.Id).ToList();

        var concluidasValidas = DisciplinasConcluidas
            .Where(id => idsDisciplinasDoCurso.Contains(id))
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
            plano = new Models.PlanoDeEstudos
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

        return RedirectToPage("/Aluno/PlanoMontado");
    }

    private async Task<ApplicationUser?> ObterAlunoAtualAsync()
    {
        return await _userManager.Users
            .Include(usuario => usuario.Curso)
            .FirstOrDefaultAsync(usuario => usuario.Id == _userManager.GetUserId(User));
    }

    private async Task CarregarDadosAsync(ApplicationUser aluno)
    {
        if (!aluno.CursoId.HasValue)
        {
            MensagemErro = "Seu curso ainda nao foi definido pelo administrador.";
            return;
        }

        NomeCurso = aluno.Curso?.Nome;

        var disciplinasFaltantes = await _context.PlanosDeEstudos
            .Where(plano => plano.AlunoId == aluno.Id)
            .SelectMany(plano => plano.Disciplinas)
            .Select(item => item.DisciplinaId)
            .ToListAsync();

        var existePlano = await _context.PlanosDeEstudos
            .AnyAsync(plano => plano.AlunoId == aluno.Id);

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

        DisciplinasObrigatorias = disciplinas
            .Where(disciplina => disciplina.Obrigatoria)
            .ToList();

        DisciplinasOptativas = disciplinas
            .Where(disciplina => !disciplina.Obrigatoria)
            .ToList();
    }
}

public record DisciplinaOpcao(
    int Id,
    string Codigo,
    string Nome,
    int CargaHoraria,
    bool Obrigatoria,
    bool Concluida);
