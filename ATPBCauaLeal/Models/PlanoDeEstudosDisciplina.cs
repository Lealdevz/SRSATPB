namespace ATPBCauaLeal.Models;

public class PlanoDeEstudosDisciplina
{
    public int PlanoDeEstudosId { get; set; }

    public PlanoDeEstudos? PlanoDeEstudos { get; set; }

    public int DisciplinaId { get; set; }

    public Disciplina? Disciplina { get; set; }
}
