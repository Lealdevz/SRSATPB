using ATPBCauaLeal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ATPBCauaLeal.Pages;

[AllowAnonymous]
public class IndexModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public IndexModel(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [BindProperty]
    public string Login { get; set; } = string.Empty;

    [BindProperty]
    public string Senha { get; set; } = string.Empty;

    public string? MensagemErro { get; set; }

    public IActionResult OnGet()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirecionarParaPainel();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var resultado = await _signInManager.PasswordSignInAsync(
            Login,
            Senha,
            isPersistent: false,
            lockoutOnFailure: false);

        if (!resultado.Succeeded)
        {
            MensagemErro = "Usuario ou senha invalidos.";
            return Page();
        }

        var usuario = await _userManager.FindByNameAsync(Login);

        return usuario is null
            ? RedirectToPage("/Index")
            : await RedirecionarParaPainelAsync(usuario);
    }

    public async Task<IActionResult> OnPostLogoutAsync()
    {
        await _signInManager.SignOutAsync();
        return RedirectToPage("/Index");
    }

    private IActionResult RedirecionarParaPainel(UserRole? role = null)
    {
        var userRole = role?.ToString();

        if (userRole is null)
        {
            if (User.IsInRole(nameof(UserRole.Admin)))
            {
                userRole = nameof(UserRole.Admin);
            }
            else if (User.IsInRole(nameof(UserRole.Aluno)))
            {
                userRole = nameof(UserRole.Aluno);
            }
            else if (User.IsInRole(nameof(UserRole.Professor)))
            {
                userRole = nameof(UserRole.Professor);
            }
        }

        return userRole switch
        {
            nameof(UserRole.Admin) => RedirectToPage("/Admin/Painel"),
            nameof(UserRole.Aluno) => RedirectToPage("/Aluno/Painel"),
            _ => RedirectToPage("/Index")
        };
    }

    private async Task<IActionResult> RedirecionarParaPainelAsync(ApplicationUser usuario)
    {
        if (await _userManager.IsInRoleAsync(usuario, nameof(UserRole.Admin)))
        {
            return RedirecionarParaPainel(UserRole.Admin);
        }

        if (await _userManager.IsInRoleAsync(usuario, nameof(UserRole.Aluno)))
        {
            return RedirecionarParaPainel(UserRole.Aluno);
        }

        if (await _userManager.IsInRoleAsync(usuario, nameof(UserRole.Professor)))
        {
            return RedirecionarParaPainel(UserRole.Professor);
        }

        return RedirectToPage("/Index");
    }
}
