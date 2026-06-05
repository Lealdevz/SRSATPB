using ATPBCauaLeal.Data;
using ATPBCauaLeal.Models;
using ATPBCauaLeal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ATPBCauaLeal.Pages.Aluno;

[Authorize(Roles = "Aluno")]
public class PlanoDeEstudosModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly PlanoDeEstudosService _planoDeEstudosService;

    public PlanoDeEstudosModel(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        PlanoDeEstudosService planoDeEstudosService)
    {
        _context = context;
        _userManager = userManager;
        _planoDeEstudosService = planoDeEstudosService;
    }

    [BindProperty]
    public List<int> DisciplinasConcluidas { get; set; } = new();

    public string? NomeCurso { get; set; }

    public List<DisciplinaOpcao> DisciplinasObrigatorias { get; set; } = new();

    public List<DisciplinaOpcao> DisciplinasOptativas { get; set; } = new();

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

        var possuiPlano = await _planoDeEstudosService.PossuiPlanoAsync(aluno.Id);

        if (possuiPlano && !editar)
        {
            return RedirectToPage("/Aluno/PlanoMontado");
        }

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

        if (!aluno.CursoId.HasValue)
        {
            MensagemErro = "Seu curso ainda não foi definido pelo administrador.";
            return RedirectToPage();
        }

        await _planoDeEstudosService.SalvarPlanoAsync(aluno, DisciplinasConcluidas);

        return RedirectToPage("/Aluno/PlanoMontado");
    }

    private async Task<ApplicationUser?> ObterAlunoAtualAsync()
    {
        return await _userManager.Users
            .Include(usuario => usuario.Curso)
            .FirstOrDefaultAsync(usuario => usuario.Id == _userManager.GetUserId(User));
    }

    private async Task CarregarDadosAsync(ApplicationUser aluno)
    {
        if (!aluno.CursoId.HasValue)
        {
            MensagemErro = "Seu curso ainda não foi definido pelo administrador.";
            return;
        }

        NomeCurso = aluno.Curso?.Nome;

        var selecao = await _planoDeEstudosService.ObterSelecaoAsync(aluno);

        DisciplinasObrigatorias = selecao.DisciplinasObrigatorias;
        DisciplinasOptativas = selecao.DisciplinasOptativas;
    }
}
