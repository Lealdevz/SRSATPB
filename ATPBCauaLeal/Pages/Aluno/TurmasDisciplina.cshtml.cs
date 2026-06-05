using ATPBCauaLeal.Data;
using ATPBCauaLeal.Models;
using ATPBCauaLeal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ATPBCauaLeal.Pages.Aluno;

[Authorize(Roles = "Aluno")]
public class TurmasDisciplinaModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly PlanoDeEstudosService _planoDeEstudosService;

    public TurmasDisciplinaModel(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        PlanoDeEstudosService planoDeEstudosService)
    {
        _context = context;
        _userManager = userManager;
        _planoDeEstudosService = planoDeEstudosService;
    }

    public string NomeDisciplina { get; set; } = string.Empty;

    public string CodigoDisciplina { get; set; } = string.Empty;

    public List<TurmaDisponivel> Turmas { get; set; } = new();

    [TempData]
    public string? MensagemErro { get; set; }

    public async Task<IActionResult> OnGetAsync(int disciplinaId)
    {
        var alunoId = _userManager.GetUserId(User);

        if (alunoId is null)
        {
            return RedirectToPage("/Index");
        }

        var disciplinaDoPlano = await _planoDeEstudosService
            .DisciplinaPertenceAoPlanoAsync(alunoId, disciplinaId);

        if (!disciplinaDoPlano)
        {
            MensagemErro = "Disciplina não encontrada no seu plano de estudos.";
            return RedirectToPage("/Aluno/PlanoMontado");
        }

        var disciplina = await _context.Disciplinas.FindAsync(disciplinaId);

        if (disciplina is null)
        {
            MensagemErro = "Disciplina não encontrada.";
            return RedirectToPage("/Aluno/PlanoMontado");
        }

        NomeDisciplina = disciplina.Nome;
        CodigoDisciplina = disciplina.Codigo;

        Turmas = await _context.Turmas
            .Include(turma => turma.Professor)
            .Where(turma => turma.DisciplinaId == disciplinaId)
            .OrderBy(turma => turma.Codigo)
            .Select(turma => new TurmaDisponivel(
                turma.Codigo,
                turma.Professor != null ? turma.Professor.Nome : "-",
                turma.DiasSemana,
                turma.Horario,
                turma.VagasDisponiveis,
                turma.Capacidade))
            .ToListAsync();

        return Page();
    }
}

public record TurmaDisponivel(
    string Codigo,
    string Professor,
    string DiasSemana,
    string Horario,
    int VagasDisponiveis,
    int Capacidade);
