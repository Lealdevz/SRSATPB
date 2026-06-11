using ATPBCauaLeal.Data;
using ATPBCauaLeal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ATPBCauaLeal.Pages.Professor;

[Authorize(Roles = "Professor")]
public class PainelModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public PainelModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public int TotalAlunos { get; set; }

    public int TotalTurmas { get; set; }

    public int TotalDisciplinas { get; set; }

    public int TotalCursos { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var professorId = _userManager.GetUserId(User);

        if (professorId is null)
        {
            return RedirectToPage("/Index");
        }

        var turmas = await _context.Turmas
            .Include(turma => turma.Disciplina)
            .Where(turma => turma.ProfessorId == professorId)
            .ToListAsync();

        TotalAlunos = await _context.Users
            .CountAsync(usuario => usuario.OrientadorId == professorId);

        TotalTurmas = turmas.Count;
        TotalDisciplinas = turmas.Select(turma => turma.DisciplinaId).Distinct().Count();
        TotalCursos = turmas
            .Where(turma => turma.Disciplina?.CursoId is not null)
            .Select(turma => turma.Disciplina!.CursoId)
            .Distinct()
            .Count();

        return Page();
    }
}
