using ATPBCauaLeal.Data;
using ATPBCauaLeal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ATPBCauaLeal.Pages.Aluno;

[Authorize(Roles = "Aluno")]
public class PlanoMontadoModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public PlanoMontadoModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public string? NomeCurso { get; set; }

    public List<DisciplinaPlano> ObrigatoriasFaltantes { get; set; } = new();

    public List<DisciplinaPlano> OptativasFaltantes { get; set; } = new();

    public int CargaHorariaFaltante { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var alunoId = _userManager.GetUserId(User);

        if (alunoId is null)
        {
            return RedirectToPage("/Index");
        }

        var plano = await _context.PlanosDeEstudos
            .Include(plano => plano.Curso)
            .Include(plano => plano.Disciplinas)
            .ThenInclude(item => item.Disciplina)
            .FirstOrDefaultAsync(plano => plano.AlunoId == alunoId);

        if (plano is null)
        {
            return RedirectToPage("/Aluno/PlanoDeEstudos");
        }

        NomeCurso = plano.Curso?.Nome;

        var disciplinasFaltantes = plano.Disciplinas
            .Where(item => item.Disciplina is not null)
            .Select(item => item.Disciplina!)
            .OrderBy(disciplina => disciplina.Nome)
            .ToList();

        ObrigatoriasFaltantes = disciplinasFaltantes
            .Where(disciplina => disciplina.Obrigatoria)
            .Select(disciplina => new DisciplinaPlano(
                disciplina.Codigo,
                disciplina.Nome,
                disciplina.CargaHoraria))
            .ToList();

        OptativasFaltantes = disciplinasFaltantes
            .Where(disciplina => !disciplina.Obrigatoria)
            .Select(disciplina => new DisciplinaPlano(
                disciplina.Codigo,
                disciplina.Nome,
                disciplina.CargaHoraria))
            .ToList();

        CargaHorariaFaltante = disciplinasFaltantes.Sum(disciplina => disciplina.CargaHoraria);

        return Page();
    }
}

public record DisciplinaPlano(string Codigo, string Nome, int CargaHoraria);
