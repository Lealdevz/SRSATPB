namespace ATPBCauaLeal.Models;

public class Turma
{
    public int Id { get; set; }

    public int DisciplinaId { get; set; }

    public int ProfessorId { get; set; }

    public string Codigo { get; set; } = string.Empty;

    public string DiasSemana { get; set; } = string.Empty;

    public string Horario { get; set; } = string.Empty;

    public int Capacidade { get; set; }

    public List<int> AlunosMatriculadosIds { get; set; } = new();

    public int VagasDisponiveis => Math.Max(0, Capacidade - AlunosMatriculadosIds.Count);

    public bool EstaCheia => VagasDisponiveis == 0;
}
