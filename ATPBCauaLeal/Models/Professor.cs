namespace ATPBCauaLeal.Models;

public class Professor
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Departamento { get; set; } = string.Empty;

    public List<int> TurmasIds { get; set; } = new();
}