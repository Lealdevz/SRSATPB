using System.ComponentModel.DataAnnotations;
using ATPBCauaLeal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ATPBCauaLeal.Pages.Admin;

[Authorize(Roles = "Admin")]
public class UsuariosModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UsuariosModel(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [BindProperty, Required(ErrorMessage = "Informe o nome.")]
    public string Nome { get; set; } = string.Empty;

    [BindProperty, Required(ErrorMessage = "Informe o login.")]
    public string Login { get; set; } = string.Empty;

    [BindProperty, Required(ErrorMessage = "Informe a senha.")]
    [MinLength(6, ErrorMessage = "A senha deve ter pelo menos 6 caracteres.")]
    public string Senha { get; set; } = string.Empty;

    [BindProperty]
    public string Perfil { get; set; } = UserRole.Aluno.ToString();

    public string[] Perfis { get; } = Enum.GetNames<UserRole>();

    public List<UsuarioResumo> Usuarios { get; set; } = new();

    [TempData]
    public string? MensagemSucesso { get; set; }

    public async Task OnGetAsync()
    {
        await CarregarUsuariosAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await CarregarUsuariosAsync();
            return Page();
        }

        var usuario = new ApplicationUser { Nome = Nome, UserName = Login };
        var resultado = await _userManager.CreateAsync(usuario, Senha);

        if (!resultado.Succeeded)
        {
            foreach (var erro in resultado.Errors)
            {
                ModelState.AddModelError(string.Empty, erro.Description);
            }

            await CarregarUsuariosAsync();
            return Page();
        }

        await _userManager.AddToRoleAsync(usuario, Perfil);

        MensagemSucesso = "Usuario cadastrado com sucesso.";
        return RedirectToPage();
    }

    private async Task CarregarUsuariosAsync()
    {
        var usuarios = await _userManager.Users.OrderBy(usuario => usuario.Nome).ToListAsync();

        foreach (var usuario in usuarios)
        {
            var perfis = await _userManager.GetRolesAsync(usuario);
            Usuarios.Add(new UsuarioResumo(usuario.Nome, usuario.UserName ?? "", perfis.FirstOrDefault() ?? "-"));
        }
    }
}

public record UsuarioResumo(string Nome, string Login, string Perfil);
