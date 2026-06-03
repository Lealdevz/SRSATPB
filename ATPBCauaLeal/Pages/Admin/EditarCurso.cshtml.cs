using System.ComponentModel.DataAnnotations;
using ATPBCauaLeal.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ATPBCauaLeal.Pages.Admin;

[Authorize(Roles = "Admin")]
public class EditarCursoModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public EditarCursoModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public int Id { get; set; }

    [BindProperty, Required(ErrorMessage = "Informe o nome do curso.")]
    public string Nome { get; set; } = string.Empty;

    [BindProperty, Required(ErrorMessage = "Informe a carga horaria minima.")]
    [Range(1, 10000, ErrorMessage = "Informe uma carga horaria valida.")]
    public int CargaHorariaMinima { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var curso = await _context.Cursos.FindAsync(id);

        if (curso is null)
        {
            return RedirectToPage("/Admin/Cursos");
        }

        Id = curso.Id;
        Nome = curso.Nome;
        CargaHorariaMinima = curso.CargaHorariaMinima;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var curso = await _context.Cursos.FindAsync(Id);

        if (curso is null)
        {
            return RedirectToPage("/Admin/Cursos");
        }

        curso.Nome = Nome;
        curso.CargaHorariaMinima = CargaHorariaMinima;

        await _context.SaveChangesAsync();

        TempData["MensagemSucesso"] = "Curso atualizado com sucesso.";
        return RedirectToPage("/Admin/Cursos");
    }
}
