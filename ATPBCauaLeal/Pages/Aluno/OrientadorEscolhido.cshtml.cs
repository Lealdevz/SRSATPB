using ATPBCauaLeal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ATPBCauaLeal.Pages.Aluno;

[Authorize(Roles = "Aluno")]
public class OrientadorEscolhidoModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;

    public OrientadorEscolhidoModel(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public string NomeOrientador { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync()
    {
        var aluno = await _userManager.GetUserAsync(User);

        if (aluno is null)
        {
            return RedirectToPage("/Index");
        }

        if (string.IsNullOrWhiteSpace(aluno.OrientadorId))
        {
            return RedirectToPage("/Aluno/Orientador");
        }

        var orientador = await _userManager.FindByIdAsync(aluno.OrientadorId);

        if (orientador is null)
        {
            return RedirectToPage("/Aluno/Orientador", new { editar = true });
        }

        NomeOrientador = orientador.Nome;
        return Page();
    }
}
