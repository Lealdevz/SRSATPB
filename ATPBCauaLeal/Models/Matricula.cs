namespace ATPBCauaLeal.Models;

public class Matricula
{
    public int Id { get; set; }

    public int AlunoId { get; set; }

    public int TurmaId { get; set; }

    public DateTime DataSolicitacao { get; set; } = DateTime.Now;

    public StatusMatricula Status { get; set; } = StatusMatricula.Pendente;
}
