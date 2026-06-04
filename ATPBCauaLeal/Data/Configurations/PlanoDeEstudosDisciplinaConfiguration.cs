using ATPBCauaLeal.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ATPBCauaLeal.Data.Configurations;

public class PlanoDeEstudosDisciplinaConfiguration : IEntityTypeConfiguration<PlanoDeEstudosDisciplina>
{
    public void Configure(EntityTypeBuilder<PlanoDeEstudosDisciplina> builder)
    {
        builder.ToTable("PlanoDeEstudosDisciplinas");

        builder.HasKey(item => new { item.PlanoDeEstudosId, item.DisciplinaId });

        builder.HasOne(item => item.PlanoDeEstudos)
            .WithMany(plano => plano.Disciplinas)
            .HasForeignKey(item => item.PlanoDeEstudosId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(item => item.Disciplina)
            .WithMany()
            .HasForeignKey(item => item.DisciplinaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
