namespace ATPBCauaLeal.Models;

public class PlanoDeEstudos
{
    public int Id { get; set; }

    public string AlunoId { get; set; } = string.Empty;

    public ApplicationUser? Aluno { get; set; }

    public int CursoId { get; set; }

    public Curso? Curso { get; set; }

    public List<PlanoDeEstudosDisciplina> Disciplinas { get; set; } = new();

    public DateTime CriadoEm { get; set; } = DateTime.Now;

    public DateTime? ConcluidoEm { get; set; }

    public bool EstaConcluido => ConcluidoEm.HasValue;
}
