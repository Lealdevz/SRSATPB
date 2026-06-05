using System.ComponentModel.DataAnnotations;
using ATPBCauaLeal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ATPBCauaLeal.Pages.Aluno;

[Authorize(Roles = "Aluno")]
public class OrientadorModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;

    public OrientadorModel(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [BindProperty, Required(ErrorMessage = "Selecione um orientador.")]
    public string OrientadorId { get; set; } = string.Empty;

    public string? NomeOrientadorAtual { get; set; }

    public List<SelectListItem> Professores { get; set; } = new();

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

        if (!string.IsNullOrWhiteSpace(aluno.OrientadorId) && !editar)
        {
            return RedirectToPage("/Aluno/OrientadorEscolhido");
        }

        OrientadorId = aluno.OrientadorId ?? string.Empty;
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

        var orientador = await _userManager.FindByIdAsync(OrientadorId);

        if (orientador is null || !await _userManager.IsInRoleAsync(orientador, nameof(UserRole.Professor)))
        {
            ModelState.AddModelError(nameof(OrientadorId), "Orientador inválido.");
        }

        if (!ModelState.IsValid)
        {
            await CarregarDadosAsync(aluno);
            return Page();
        }

        aluno.OrientadorId = OrientadorId;
        await _userManager.UpdateAsync(aluno);

        MensagemSucesso = "Orientador escolhido com sucesso.";
        return RedirectToPage("/Aluno/OrientadorEscolhido");
    }

    private async Task<ApplicationUser?> ObterAlunoAtualAsync()
    {
        return await _userManager.GetUserAsync(User);
    }

    private async Task CarregarDadosAsync(ApplicationUser aluno)
    {
        var professores = await _userManager.GetUsersInRoleAsync(nameof(UserRole.Professor));

        Professores = professores
            .OrderBy(professor => professor.Nome)
            .Select(professor => new SelectListItem(professor.Nome, professor.Id))
            .ToList();

        if (!string.IsNullOrWhiteSpace(aluno.OrientadorId))
        {
            var orientadorAtual = await _userManager.FindByIdAsync(aluno.OrientadorId);
            NomeOrientadorAtual = orientadorAtual?.Nome;
        }

        if (Professores.Count == 0)
        {
            MensagemErro = "Ainda não existem professores cadastrados.";
        }
    }
}
