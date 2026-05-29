using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ATPBCauaLeal.Pages.Admin;

[Authorize(Roles = "Admin")]
public class PainelModel : PageModel
{
    public void OnGet()
    {
    }
}
