namespace ATPBCauaLeal.Models;

public class PlanoDeEstudos
{
    public int Id { get; set; }

    public int AlunoId { get; set; }

    public int CursoId { get; set; }

    public List<int> DisciplinasEscolhidasIds { get; set; } = new();

    public DateTime CriadoEm { get; set; } = DateTime.Now;

    public DateTime? ConcluidoEm { get; set; }

    public bool EstaConcluido => ConcluidoEm.HasValue;
}
