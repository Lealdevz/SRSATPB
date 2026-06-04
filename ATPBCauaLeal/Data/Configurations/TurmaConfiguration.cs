using ATPBCauaLeal.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ATPBCauaLeal.Data.Configurations;

public class TurmaConfiguration : IEntityTypeConfiguration<Turma>
{
    public void Configure(EntityTypeBuilder<Turma> builder)
    {
        builder.ToTable("Turmas");

        builder.HasKey(turma => turma.Id);

        builder.Property(turma => turma.Codigo)
            .IsRequired();

        builder.Property(turma => turma.DiasSemana)
            .IsRequired();

        builder.Property(turma => turma.Horario)
            .IsRequired();

        builder.Property(turma => turma.Capacidade)
            .IsRequired();

        builder.HasOne(turma => turma.Disciplina)
            .WithMany()
            .HasForeignKey(turma => turma.DisciplinaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(turma => turma.Professor)
            .WithMany()
            .HasForeignKey(turma => turma.ProfessorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(turma => turma.AlunosMatriculadosIds);
        builder.Ignore(turma => turma.VagasDisponiveis);
        builder.Ignore(turma => turma.EstaCheia);
    }
}
