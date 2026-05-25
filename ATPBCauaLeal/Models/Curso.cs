namespace ATPBCauaLeal.Models;

public class Curso
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public int CargaHorariaMinima { get; set; }

    public List<int> DisciplinasObrigatoriasIds { get; set; } = new();

    public List<int> DisciplinasOptativasIds { get; set; } = new();
}
