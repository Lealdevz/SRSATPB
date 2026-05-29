using Microsoft.AspNetCore.Identity;

namespace ATPBCauaLeal.Models;

public class ApplicationUser : IdentityUser
{
    public string Nome { get; set; } = string.Empty;
}
