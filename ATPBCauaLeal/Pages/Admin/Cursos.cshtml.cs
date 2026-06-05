using System.ComponentModel.DataAnnotations;
using ATPBCauaLeal.Data;
using ATPBCauaLeal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ATPBCauaLeal.Pages.Admin;

[Authorize(Roles = "Admin")]
public class CursosModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CursosModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty, Required(ErrorMessage = "Informe o nome do curso.")]
    public string Nome { get; set; } = string.Empty;

    [BindProperty, Required(ErrorMessage = "Informe a carga horária mínima.")]
    [Range(1, 10000, ErrorMessage = "Informe uma carga horária válida.")]
    public int CargaHorariaMinima { get; set; }

    public List<Curso> Cursos { get; set; } = new();

    [TempData]
    public string? MensagemSucesso { get; set; }

    [TempData]
    public string? MensagemErro { get; set; }

    public async Task OnGetAsync()
    {
        await CarregarCursosAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await CarregarCursosAsync();
            return Page();
        }

        var curso = new Curso
        {
            Nome = Nome,
            CargaHorariaMinima = CargaHorariaMinima
        };

        _context.Cursos.Add(curso);
        await _context.SaveChangesAsync();

        MensagemSucesso = "Curso cadastrado com sucesso.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostExcluirAsync(int id)
    {
        var curso = await _context.Cursos.FindAsync(id);

        if (curso is not null)
        {
            var possuiDisciplinas = await _context.Disciplinas
                .AnyAsync(disciplina => disciplina.CursoId == id);

            if (possuiDisciplinas)
            {
                MensagemErro = "Não é possível excluir um curso que possui disciplinas cadastradas.";
                return RedirectToPage();
            }

            _context.Cursos.Remove(curso);
            await _context.SaveChangesAsync();
            MensagemSucesso = "Curso excluído com sucesso.";
        }

        return RedirectToPage();
    }

    private async Task CarregarCursosAsync()
    {
        Cursos = await _context.Cursos
            .OrderBy(curso => curso.Nome)
            .ToListAsync();
    }
}
