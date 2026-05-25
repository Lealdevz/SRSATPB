namespace ATPBCauaLeal.Models;

public class Aluno
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Curso { get; set; } = string.Empty;

    public int? OrientadorId { get; set; }

    public bool PossuiPlanoEstudos { get; set; }

    public int QuantidadeDisciplinasPlano { get; set; }

    public string StatusMatricula { get; set; } = "Inativa";
}