using ATPBCauaLeal.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ATPBCauaLeal.Data.Configurations;

public class PlanoDeEstudosConfiguration : IEntityTypeConfiguration<PlanoDeEstudos>
{
    public void Configure(EntityTypeBuilder<PlanoDeEstudos> builder)
    {
        builder.ToTable("PlanosDeEstudos");

        builder.HasKey(plano => plano.Id);

        builder.Property(plano => plano.AlunoId)
            .IsRequired();

        builder.Property(plano => plano.CriadoEm)
            .IsRequired();

        builder.HasOne(plano => plano.Aluno)
            .WithMany()
            .HasForeignKey(plano => plano.AlunoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(plano => plano.Curso)
            .WithMany()
            .HasForeignKey(plano => plano.CursoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(plano => plano.AlunoId)
            .IsUnique();

        builder.Ignore(plano => plano.EstaConcluido);
    }
}
