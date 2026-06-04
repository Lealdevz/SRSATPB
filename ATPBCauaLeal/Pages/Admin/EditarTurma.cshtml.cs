using System.ComponentModel.DataAnnotations;
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
public class EditarTurmaModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public EditarTurmaModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [BindProperty]
    public int Id { get; set; }

    [BindProperty, Required(ErrorMessage = "Selecione a disciplina.")]
    public int DisciplinaId { get; set; }

    [BindProperty, Required(ErrorMessage = "Selecione o professor.")]
    public string ProfessorId { get; set; } = string.Empty;

    [BindProperty, Required(ErrorMessage = "Informe o codigo.")]
    public string Codigo { get; set; } = string.Empty;

    [BindProperty, Required(ErrorMessage = "Informe os dias da semana.")]
    public string DiasSemana { get; set; } = string.Empty;

    [BindProperty, Required(ErrorMessage = "Informe o horario.")]
    public string Horario { get; set; } = string.Empty;

    [BindProperty, Required(ErrorMessage = "Informe a capacidade.")]
    [Range(1, 500, ErrorMessage = "Informe uma capacidade valida.")]
    public int Capacidade { get; set; }

    public List<SelectListItem> Disciplinas { get; set; } = new();

    public List<SelectListItem> Professores { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var turma = await _context.Turmas.FindAsync(id);

        if (turma is null)
        {
            return RedirectToPage("/Admin/Turmas");
        }

        Id = turma.Id;
        DisciplinaId = turma.DisciplinaId;
        ProfessorId = turma.ProfessorId;
        Codigo = turma.Codigo;
        DiasSemana = turma.DiasSemana;
        Horario = turma.Horario;
        Capacidade = turma.Capacidade;

        await CarregarOpcoesAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await ValidarRelacionamentosAsync();

        if (!ModelState.IsValid)
        {
            await CarregarOpcoesAsync();
            return Page();
        }

        var turma = await _context.Turmas.FindAsync(Id);

        if (turma is null)
        {
            return RedirectToPage("/Admin/Turmas");
        }

        turma.DisciplinaId = DisciplinaId;
        turma.ProfessorId = ProfessorId;
        turma.Codigo = Codigo;
        turma.DiasSemana = DiasSemana;
        turma.Horario = Horario;
        turma.Capacidade = Capacidade;

        await _context.SaveChangesAsync();

        TempData["MensagemSucesso"] = "Turma atualizada com sucesso.";
        return RedirectToPage("/Admin/Turmas");
    }

    private async Task ValidarRelacionamentosAsync()
    {
        if (!await _context.Disciplinas.AnyAsync(disciplina => disciplina.Id == DisciplinaId))
        {
            ModelState.AddModelError(nameof(DisciplinaId), "Disciplina invalida.");
        }

        var professor = await _userManager.FindByIdAsync(ProfessorId);

        if (professor is null || !await _userManager.IsInRoleAsync(professor, nameof(UserRole.Professor)))
        {
            ModelState.AddModelError(nameof(ProfessorId), "Professor invalido.");
        }
    }

    private async Task CarregarOpcoesAsync()
    {
        Disciplinas = await _context.Disciplinas
            .Include(disciplina => disciplina.Curso)
            .OrderBy(disciplina => disciplina.Curso!.Nome)
            .ThenBy(disciplina => disciplina.Nome)
            .Select(disciplina => new SelectListItem(
                disciplina.Curso!.Nome + " - " + disciplina.Codigo + " - " + disciplina.Nome,
                disciplina.Id.ToString()))
            .ToListAsync();

        var professores = await _userManager.GetUsersInRoleAsync(nameof(UserRole.Professor));

        Professores = professores
            .OrderBy(professor => professor.Nome)
            .Select(professor => new SelectListItem(professor.Nome, professor.Id))
            .ToList();
    }
}
