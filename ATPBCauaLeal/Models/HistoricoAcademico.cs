namespace ATPBCauaLeal.Models;

public class HistoricoAcademico
{
    public int Id { get; set; }

    public int AlunoId { get; set; }

    public List<int> DisciplinasConcluidasIds { get; set; } = new();

    public bool PossuiPendenciaFinanceira { get; set; }

    public bool ConcluiuDisciplina(int disciplinaId)
    {
        return DisciplinasConcluidasIds.Contains(disciplinaId);
    }
}
