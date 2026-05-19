using Fidelis.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fidelis.Infrastructure.Persistence.Configurations;

public class ConsultaConfiguration : IEntityTypeConfiguration<Consulta>
{
    public void Configure(EntityTypeBuilder<Consulta> builder)
    {
        builder.ToTable("Consultas");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.DataHora)
            .IsRequired();

        builder.Property(x => x.Tipo)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Diagnostico);
        builder.Property(x => x.Observacoes);
        builder.Property(x => x.DataRetorno);

        builder.HasOne(x => x.Veterinario)
            .WithMany(x => x.Consultas)
            .HasForeignKey(x => x.VeterinarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Pet)
            .WithMany(x => x.Consultas)
            .HasForeignKey(x => x.PetId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Exames)
            .WithOne(x => x.Consulta)
            .HasForeignKey(x => x.ConsultaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Prescricoes)
            .WithOne(x => x.Consulta)
            .HasForeignKey(x => x.ConsultaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

