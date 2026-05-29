namespace ATPBCauaLeal.Models;

public class Admin
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public bool PodeGerenciarUsuarios { get; set; } = true;

    public bool PodeGerenciarTurmas { get; set; } = true;
}
