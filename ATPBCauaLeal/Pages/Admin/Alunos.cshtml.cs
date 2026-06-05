using ATPBCauaLeal.Data;
using ATPBCauaLeal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ATPBCauaLeal.Pages.Admin;

[Authorize(Roles = "Admin")]
public class AlunosModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public AlunosModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [BindProperty]
    public string AlunoId { get; set; } = string.Empty;

    [BindProperty]
    public int? CursoId { get; set; }

    public List<SelectListItem> Cursos { get; set; } = new();

    public List<AlunoResumo> Alunos { get; set; } = new();

    [TempData]
    public string? MensagemSucesso { get; set; }

    [TempData]
    public string? MensagemErro { get; set; }

    public async Task OnGetAsync()
    {
        await CarregarDadosAsync();
    }

    public async Task<IActionResult> OnPostSalvarCursoAsync()
    {
        var aluno = await _userManager.FindByIdAsync(AlunoId);

        if (aluno is null || !await _userManager.IsInRoleAsync(aluno, nameof(UserRole.Aluno)))
        {
            MensagemErro = "Aluno inválido.";
            return RedirectToPage();
        }

        if (CursoId.HasValue && !await _context.Cursos.AnyAsync(curso => curso.Id == CursoId.Value))
        {
            MensagemErro = "Curso inválido.";
            return RedirectToPage();
        }

        aluno.CursoId = CursoId;
        await _userManager.UpdateAsync(aluno);

        MensagemSucesso = "Curso do aluno atualizado com sucesso.";
        return RedirectToPage();
    }

    private async Task CarregarDadosAsync()
    {
        Cursos = await _context.Cursos
            .OrderBy(curso => curso.Nome)
            .Select(curso => new SelectListItem(curso.Nome, curso.Id.ToString()))
            .ToListAsync();

        var alunos = await _userManager.GetUsersInRoleAsync(nameof(UserRole.Aluno));
        var alunosIds = alunos.Select(aluno => aluno.Id).ToList();

        Alunos = await _context.Users
            .Include(usuario => usuario.Curso)
            .Where(usuario => alunosIds.Contains(usuario.Id))
            .OrderBy(usuario => usuario.Nome)
            .Select(usuario => new AlunoResumo(
                usuario.Id,
                usuario.Nome,
                usuario.UserName ?? "",
                usuario.CursoId,
                usuario.Curso != null ? usuario.Curso.Nome : "-"))
            .ToListAsync();
    }
}

public record AlunoResumo(string Id, string Nome, string Login, int? CursoId, string Curso);
