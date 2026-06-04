using Microsoft.AspNetCore.Identity;

namespace ATPBCauaLeal.Models;

public class ApplicationUser : IdentityUser
{
    public string Nome { get; set; } = string.Empty;

    public string? OrientadorId { get; set; }

    public ApplicationUser? Orientador { get; set; }

    public int? CursoId { get; set; }

    public Curso? Curso { get; set; }
}
