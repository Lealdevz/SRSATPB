namespace ATPBCauaLeal.Models;

public class Turma
{
    public int Id { get; set; }

    public int DisciplinaId { get; set; }

    public Disciplina? Disciplina { get; set; }

    public string ProfessorId { get; set; } = string.Empty;

    public ApplicationUser? Professor { get; set; }

    public string Codigo { get; set; } = string.Empty;

    public string DiasSemana { get; set; } = string.Empty;

    public string Horario { get; set; } = string.Empty;

    public int Capacidade { get; set; }

    public int VagasDisponiveis => Capacidade;

    public bool EstaCheia => VagasDisponiveis == 0;
}
