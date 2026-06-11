using ATPBCauaLeal.Data;
using ATPBCauaLeal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ATPBCauaLeal.Pages.Professor;

[Authorize(Roles = "Professor")]
public class AlunosModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public AlunosModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public List<AlunoOrientadoItem> Alunos { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var professorId = _userManager.GetUserId(User);

        if (professorId is null)
        {
            return RedirectToPage("/Index");
        }

        Alunos = await _context.Users
            .Include(usuario => usuario.Curso)
            .Where(usuario => usuario.OrientadorId == professorId)
            .OrderBy(usuario => usuario.Nome)
            .Select(usuario => new AlunoOrientadoItem
            {
                Nome = usuario.Nome,
                Login = usuario.UserName ?? string.Empty,
                Curso = usuario.Curso != null ? usuario.Curso.Nome : "Não definido",
                PossuiPlanoDeEstudo = _context.PlanosDeEstudos
                    .Any(plano => plano.AlunoId == usuario.Id)
            })
            .ToListAsync();

        return Page();
    }
}

public class AlunoOrientadoItem
{
    public string Nome { get; set; } = string.Empty;

    public string Login { get; set; } = string.Empty;

    public string Curso { get; set; } = string.Empty;

    public bool PossuiPlanoDeEstudo { get; set; }
}
