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
public class PainelModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly PlanoDeEstudosService _planoDeEstudosService;

    public PainelModel(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        PlanoDeEstudosService planoDeEstudosService)
    {
        _context = context;
        _userManager = userManager;
        _planoDeEstudosService = planoDeEstudosService;
    }

    public bool PossuiPlano { get; set; }

    public int DisciplinasConcluidas { get; set; }

    public int DisciplinasFaltantes { get; set; }

    public string NomeOrientador { get; set; } = "Não definido";

    public async Task<IActionResult> OnGetAsync()
    {
        var aluno = await _userManager.Users
            .Include(usuario => usuario.Orientador)
            .FirstOrDefaultAsync(usuario => usuario.Id == _userManager.GetUserId(User));

        if (aluno is null)
        {
            return RedirectToPage("/Index");
        }

        NomeOrientador = aluno.Orientador?.Nome ?? "Não definido";
        PossuiPlano = await _planoDeEstudosService.PossuiPlanoAsync(aluno.Id);

        if (aluno.CursoId.HasValue && PossuiPlano)
        {
            var totalDisciplinas = await _context.Disciplinas
                .CountAsync(disciplina => disciplina.CursoId == aluno.CursoId.Value);

            var plano = await _planoDeEstudosService.ObterPlanoMontadoAsync(aluno.Id);

            DisciplinasFaltantes =
                (plano?.ObrigatoriasFaltantes.Count ?? 0) +
                (plano?.OptativasFaltantes.Count ?? 0);

            DisciplinasConcluidas = Math.Max(0, totalDisciplinas - DisciplinasFaltantes);
        }

        return Page();
    }
}
