using ATPBCauaLeal.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ATPBCauaLeal.Data.Configurations;

public class DisciplinaConfiguration : IEntityTypeConfiguration<Disciplina>
{
    public void Configure(EntityTypeBuilder<Disciplina> builder)
    {
        builder.ToTable("Disciplinas");

        builder.HasKey(disciplina => disciplina.Id);

        builder.Property(disciplina => disciplina.Codigo)
            .IsRequired();

        builder.Property(disciplina => disciplina.Nome)
            .IsRequired();

        builder.Property(disciplina => disciplina.CargaHoraria)
            .IsRequired();

        builder.Property(disciplina => disciplina.Obrigatoria)
            .IsRequired();

        builder.HasOne(disciplina => disciplina.Curso)
            .WithMany()
            .HasForeignKey(disciplina => disciplina.CursoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(disciplina => disciplina.PreRequisitosIds);
    }
}
