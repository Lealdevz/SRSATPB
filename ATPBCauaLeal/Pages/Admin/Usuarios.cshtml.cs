using System.ComponentModel.DataAnnotations;
using ATPBCauaLeal.Data;
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
    private readonly ApplicationDbContext _context;

    public UsuariosModel(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
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

    [TempData]
    public string? MensagemErro { get; set; }

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

        MensagemSucesso = "Usuário cadastrado com sucesso.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostExcluirAsync(string id)
    {
        var usuario = await _userManager.FindByIdAsync(id);

        if (usuario is null)
        {
            return RedirectToPage();
        }

        if (usuario.Id == _userManager.GetUserId(User))
        {
            MensagemErro = "Você não pode excluir o próprio usuário.";
            return RedirectToPage();
        }

        var possuiTurmas = await _context.Turmas
            .AnyAsync(turma => turma.ProfessorId == usuario.Id);

        if (possuiTurmas)
        {
            MensagemErro = "Não é possível excluir um professor que possui turmas cadastradas.";
            return RedirectToPage();
        }

        var possuiOrientandos = await _context.Users
            .AnyAsync(aluno => aluno.OrientadorId == usuario.Id);

        if (possuiOrientandos)
        {
            MensagemErro = "Não é possível excluir um professor que possui alunos orientandos.";
            return RedirectToPage();
        }

        await _userManager.DeleteAsync(usuario);

        MensagemSucesso = "Usuário excluído com sucesso.";
        return RedirectToPage();
    }

    private async Task CarregarUsuariosAsync()
    {
        var usuarios = await _userManager.Users.OrderBy(usuario => usuario.Nome).ToListAsync();

        foreach (var usuario in usuarios)
        {
            var perfis = await _userManager.GetRolesAsync(usuario);
            Usuarios.Add(new UsuarioResumo(usuario.Id, usuario.Nome, usuario.UserName ?? "", perfis.FirstOrDefault() ?? "-"));
        }
    }
}

public record UsuarioResumo(string Id, string Nome, string Login, string Perfil);
