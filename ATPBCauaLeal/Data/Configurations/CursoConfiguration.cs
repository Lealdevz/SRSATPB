using ATPBCauaLeal.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ATPBCauaLeal.Data.Configurations;

public class CursoConfiguration : IEntityTypeConfiguration<Curso>
{
    public void Configure(EntityTypeBuilder<Curso> builder)
    {
        builder.ToTable("Cursos");

        builder.HasKey(curso => curso.Id);

        builder.Property(curso => curso.Nome)
            .IsRequired();

        builder.Property(curso => curso.CargaHorariaMinima)
            .IsRequired();

        builder.Ignore(curso => curso.DisciplinasObrigatoriasIds);
        builder.Ignore(curso => curso.DisciplinasOptativasIds);
    }
}
