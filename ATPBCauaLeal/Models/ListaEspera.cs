namespace ATPBCauaLeal.Models;

public class ListaEspera
{
    public int Id { get; set; }

    public int AlunoId { get; set; }

    public int TurmaId { get; set; }

    public int Posicao { get; set; }

    public DateTime EntrouEm { get; set; } = DateTime.Now;

    public bool NotificadoPorEmail { get; set; }
}
