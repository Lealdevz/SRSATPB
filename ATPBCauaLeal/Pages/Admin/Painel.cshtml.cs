using ATPBCauaLeal.Data;
using ATPBCauaLeal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ATPBCauaLeal.Pages.Admin;

[Authorize(Roles = "Admin")]
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

    public int TotalProfessores { get; set; }

    public int TotalAdministradores { get; set; }

    public int TotalCursos { get; set; }

    public int TotalDisciplinas { get; set; }

    public int TotalTurmas { get; set; }

    public int TotalPlanos { get; set; }

    public async Task OnGetAsync()
    {
        TotalAlunos = (await _userManager.GetUsersInRoleAsync(nameof(UserRole.Aluno))).Count;
        TotalProfessores = (await _userManager.GetUsersInRoleAsync(nameof(UserRole.Professor))).Count;
        TotalAdministradores = (await _userManager.GetUsersInRoleAsync(nameof(UserRole.Admin))).Count;

        TotalCursos = await _context.Cursos.CountAsync();
        TotalDisciplinas = await _context.Disciplinas.CountAsync();
        TotalTurmas = await _context.Turmas.CountAsync();
        TotalPlanos = await _context.PlanosDeEstudos.CountAsync();
    }
}
