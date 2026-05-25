namespace ATPBCauaLeal.Models;

public class Disciplina
{
    public int Id { get; set; }

    public string Codigo { get; set; } = string.Empty;

    public string Nome { get; set; } = string.Empty;

    public int CargaHoraria { get; set; }

    public bool Obrigatoria { get; set; } = true;

    public List<int> PreRequisitosIds { get; set; } = new();
}
