using ATPBCauaLeal.Data;
using ATPBCauaLeal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ATPBCauaLeal.Pages.Professor;

[Authorize(Roles = "Professor")]
public class TurmasModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public TurmasModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public List<Turma> Turmas { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var professorId = _userManager.GetUserId(User);

        if (professorId is null)
        {
            return RedirectToPage("/Index");
        }

        Turmas = await _context.Turmas
            .Include(turma => turma.Disciplina)
            .ThenInclude(disciplina => disciplina!.Curso)
            .Where(turma => turma.ProfessorId == professorId)
            .OrderBy(turma => turma.Disciplina!.Curso!.Nome)
            .ThenBy(turma => turma.Disciplina!.Nome)
            .ThenBy(turma => turma.Codigo)
            .ToListAsync();

        return Page();
    }
}
