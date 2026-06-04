using ATPBCauaLeal.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ATPBCauaLeal.Data.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.HasOne(usuario => usuario.Orientador)
            .WithMany()
            .HasForeignKey(usuario => usuario.OrientadorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(usuario => usuario.Curso)
            .WithMany()
            .HasForeignKey(usuario => usuario.CursoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
