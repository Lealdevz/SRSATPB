using System.ComponentModel.DataAnnotations;
using ATPBCauaLeal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ATPBCauaLeal.Pages.Admin;

[Authorize(Roles = "Admin")]
public class EditarUsuarioModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;

    public EditarUsuarioModel(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [BindProperty]
    public string Id { get; set; } = string.Empty;

    [BindProperty, Required(ErrorMessage = "Informe o nome.")]
    public string Nome { get; set; } = string.Empty;

    [BindProperty, Required(ErrorMessage = "Informe o login.")]
    public string Login { get; set; } = string.Empty;

    [BindProperty]
    [MinLength(6, ErrorMessage = "A nova senha deve ter pelo menos 6 caracteres.")]
    public string? NovaSenha { get; set; }

    public async Task<IActionResult> OnGetAsync(string id)
    {
        var usuario = await _userManager.FindByIdAsync(id);

        if (usuario is null)
        {
            return RedirectToPage("/Admin/Usuarios");
        }

        Id = usuario.Id;
        Nome = usuario.Nome;
        Login = usuario.UserName ?? string.Empty;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var usuario = await _userManager.FindByIdAsync(Id);

        if (usuario is null)
        {
            return RedirectToPage("/Admin/Usuarios");
        }

        usuario.Nome = Nome;
        usuario.UserName = Login;

        var resultado = await _userManager.UpdateAsync(usuario);

        if (!resultado.Succeeded)
        {
            AdicionarErros(resultado);
            return Page();
        }

        if (!string.IsNullOrWhiteSpace(NovaSenha))
        {
            await _userManager.RemovePasswordAsync(usuario);
            resultado = await _userManager.AddPasswordAsync(usuario, NovaSenha);

            if (!resultado.Succeeded)
            {
                AdicionarErros(resultado);
                return Page();
            }
        }

        TempData["MensagemSucesso"] = "Usuario atualizado com sucesso.";
        return RedirectToPage("/Admin/Usuarios");
    }

    private void AdicionarErros(IdentityResult resultado)
    {
        foreach (var erro in resultado.Errors)
        {
            ModelState.AddModelError(string.Empty, erro.Description);
        }
    }
}
