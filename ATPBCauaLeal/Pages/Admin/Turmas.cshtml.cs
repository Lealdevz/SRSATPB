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
public class TurmasModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public TurmasModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [BindProperty, Required(ErrorMessage = "Selecione a disciplina.")]
    public int DisciplinaId { get; set; }

    [BindProperty, Required(ErrorMessage = "Selecione o professor.")]
    public string ProfessorId { get; set; } = string.Empty;

    [BindProperty, Required(ErrorMessage = "Informe o código.")]
    public string Codigo { get; set; } = string.Empty;

    [BindProperty, Required(ErrorMessage = "Informe os dias da semana.")]
    public string DiasSemana { get; set; } = string.Empty;

    [BindProperty, Required(ErrorMessage = "Informe o horário.")]
    public string Horario { get; set; } = string.Empty;

    [BindProperty, Required(ErrorMessage = "Informe a capacidade.")]
    [Range(1, 500, ErrorMessage = "Informe uma capacidade válida.")]
    public int Capacidade { get; set; }

    public List<SelectListItem> Disciplinas { get; set; } = new();

    public List<SelectListItem> Professores { get; set; } = new();

    public List<Turma> Turmas { get; set; } = new();

    [TempData]
    public string? MensagemSucesso { get; set; }

    [TempData]
    public string? MensagemErro { get; set; }

    public async Task OnGetAsync()
    {
        await CarregarDadosAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await ValidarRelacionamentosAsync();

        if (!ModelState.IsValid)
        {
            await CarregarDadosAsync();
            return Page();
        }

        var turma = new Turma
        {
            DisciplinaId = DisciplinaId,
            ProfessorId = ProfessorId,
            Codigo = Codigo,
            DiasSemana = DiasSemana,
            Horario = Horario,
            Capacidade = Capacidade
        };

        _context.Turmas.Add(turma);
        await _context.SaveChangesAsync();

        MensagemSucesso = "Turma cadastrada com sucesso.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostExcluirAsync(int id)
    {
        var turma = await _context.Turmas.FindAsync(id);

        if (turma is not null)
        {
            _context.Turmas.Remove(turma);
            await _context.SaveChangesAsync();
            MensagemSucesso = "Turma excluída com sucesso.";
        }

        return RedirectToPage();
    }

    private async Task ValidarRelacionamentosAsync()
    {
        if (!await _context.Disciplinas.AnyAsync(disciplina => disciplina.Id == DisciplinaId))
        {
            ModelState.AddModelError(nameof(DisciplinaId), "Disciplina inválida.");
        }

        var professor = await _userManager.FindByIdAsync(ProfessorId);

        if (professor is null || !await _userManager.IsInRoleAsync(professor, nameof(UserRole.Professor)))
        {
            ModelState.AddModelError(nameof(ProfessorId), "Professor inválido.");
        }
    }

    private async Task CarregarDadosAsync()
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

        Turmas = await _context.Turmas
            .Include(turma => turma.Disciplina)
            .ThenInclude(disciplina => disciplina!.Curso)
            .Include(turma => turma.Professor)
            .OrderBy(turma => turma.Disciplina!.Curso!.Nome)
            .ThenBy(turma => turma.Disciplina!.Nome)
            .ThenBy(turma => turma.Codigo)
            .ToListAsync();
    }
}
