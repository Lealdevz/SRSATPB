using ATPBCauaLeal.Models;
using ATPBCauaLeal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ATPBCauaLeal.Pages.Aluno;

[Authorize(Roles = "Aluno")]
public class PlanoMontadoModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly PlanoDeEstudosService _planoDeEstudosService;

    public PlanoMontadoModel(
        UserManager<ApplicationUser> userManager,
        PlanoDeEstudosService planoDeEstudosService)
    {
        _userManager = userManager;
        _planoDeEstudosService = planoDeEstudosService;
    }

    public string? NomeCurso { get; set; }

    public List<DisciplinaPlano> ObrigatoriasFaltantes { get; set; } = new();

    public List<DisciplinaPlano> OptativasFaltantes { get; set; } = new();

    public int CargaHorariaFaltante { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var alunoId = _userManager.GetUserId(User);

        if (alunoId is null)
        {
            return RedirectToPage("/Index");
        }

        var plano = await _planoDeEstudosService.ObterPlanoMontadoAsync(alunoId);

        if (plano is null)
        {
            return RedirectToPage("/Aluno/PlanoDeEstudos");
        }

        NomeCurso = plano.NomeCurso;
        ObrigatoriasFaltantes = plano.ObrigatoriasFaltantes;
        OptativasFaltantes = plano.OptativasFaltantes;
        CargaHorariaFaltante = plano.CargaHorariaFaltante;

        return Page();
    }
}
