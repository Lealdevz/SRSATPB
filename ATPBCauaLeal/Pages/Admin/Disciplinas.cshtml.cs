using System.ComponentModel.DataAnnotations;
using ATPBCauaLeal.Data;
using ATPBCauaLeal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ATPBCauaLeal.Pages.Admin;

[Authorize(Roles = "Admin")]
public class DisciplinasModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DisciplinasModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty, Required(ErrorMessage = "Selecione o curso.")]
    public int CursoId { get; set; }

    [BindProperty, Required(ErrorMessage = "Informe o codigo.")]
    public string Codigo { get; set; } = string.Empty;

    [BindProperty, Required(ErrorMessage = "Informe o nome.")]
    public string Nome { get; set; } = string.Empty;

    [BindProperty, Required(ErrorMessage = "Informe a carga horária.")]
    [Range(1, 1000, ErrorMessage = "Informe uma carga horária válida.")]
    public int CargaHoraria { get; set; }

    [BindProperty]
    public bool Obrigatoria { get; set; } = true;

    public List<SelectListItem> Cursos { get; set; } = new();

    public List<Disciplina> Disciplinas { get; set; } = new();

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
        if (!await _context.Cursos.AnyAsync(curso => curso.Id == CursoId))
        {
            ModelState.AddModelError(nameof(CursoId), "Curso inválido.");
        }

        if (!ModelState.IsValid)
        {
            await CarregarDadosAsync();
            return Page();
        }

        var disciplina = new Disciplina
        {
            CursoId = CursoId,
            Codigo = Codigo,
            Nome = Nome,
            CargaHoraria = CargaHoraria,
            Obrigatoria = Obrigatoria
        };

        _context.Disciplinas.Add(disciplina);
        await _context.SaveChangesAsync();

        MensagemSucesso = "Disciplina cadastrada com sucesso.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostExcluirAsync(int id)
    {
        var disciplina = await _context.Disciplinas.FindAsync(id);

        if (disciplina is not null)
        {
            var possuiTurmas = await _context.Turmas
                .AnyAsync(turma => turma.DisciplinaId == id);

            if (possuiTurmas)
            {
                MensagemErro = "Não é possível excluir uma disciplina que possui turmas cadastradas.";
                return RedirectToPage();
            }

            _context.Disciplinas.Remove(disciplina);
            await _context.SaveChangesAsync();
            MensagemSucesso = "Disciplina excluída com sucesso.";
        }

        return RedirectToPage();
    }

    private async Task CarregarDadosAsync()
    {
        Cursos = await _context.Cursos
            .OrderBy(curso => curso.Nome)
            .Select(curso => new SelectListItem(curso.Nome, curso.Id.ToString()))
            .ToListAsync();

        Disciplinas = await _context.Disciplinas
            .Include(disciplina => disciplina.Curso)
            .OrderBy(disciplina => disciplina.Curso!.Nome)
            .ThenBy(disciplina => disciplina.Nome)
            .ToListAsync();
    }
}
