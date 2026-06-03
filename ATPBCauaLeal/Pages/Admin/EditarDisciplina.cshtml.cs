using System.ComponentModel.DataAnnotations;
using ATPBCauaLeal.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ATPBCauaLeal.Pages.Admin;

[Authorize(Roles = "Admin")]
public class EditarDisciplinaModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public EditarDisciplinaModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public int Id { get; set; }

    [BindProperty, Required(ErrorMessage = "Selecione o curso.")]
    public int CursoId { get; set; }

    [BindProperty, Required(ErrorMessage = "Informe o codigo.")]
    public string Codigo { get; set; } = string.Empty;

    [BindProperty, Required(ErrorMessage = "Informe o nome.")]
    public string Nome { get; set; } = string.Empty;

    [BindProperty, Required(ErrorMessage = "Informe a carga horaria.")]
    [Range(1, 1000, ErrorMessage = "Informe uma carga horaria valida.")]
    public int CargaHoraria { get; set; }

    [BindProperty]
    public bool Obrigatoria { get; set; }

    public List<SelectListItem> Cursos { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var disciplina = await _context.Disciplinas.FindAsync(id);

        if (disciplina is null)
        {
            return RedirectToPage("/Admin/Disciplinas");
        }

        Id = disciplina.Id;
        CursoId = disciplina.CursoId;
        Codigo = disciplina.Codigo;
        Nome = disciplina.Nome;
        CargaHoraria = disciplina.CargaHoraria;
        Obrigatoria = disciplina.Obrigatoria;

        await CarregarCursosAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!await _context.Cursos.AnyAsync(curso => curso.Id == CursoId))
        {
            ModelState.AddModelError(nameof(CursoId), "Curso invalido.");
        }

        if (!ModelState.IsValid)
        {
            await CarregarCursosAsync();
            return Page();
        }

        var disciplina = await _context.Disciplinas.FindAsync(Id);

        if (disciplina is null)
        {
            return RedirectToPage("/Admin/Disciplinas");
        }

        disciplina.CursoId = CursoId;
        disciplina.Codigo = Codigo;
        disciplina.Nome = Nome;
        disciplina.CargaHoraria = CargaHoraria;
        disciplina.Obrigatoria = Obrigatoria;

        await _context.SaveChangesAsync();

        TempData["MensagemSucesso"] = "Disciplina atualizada com sucesso.";
        return RedirectToPage("/Admin/Disciplinas");
    }

    private async Task CarregarCursosAsync()
    {
        Cursos = await _context.Cursos
            .OrderBy(curso => curso.Nome)
            .Select(curso => new SelectListItem(curso.Nome, curso.Id.ToString()))
            .ToListAsync();
    }
}
