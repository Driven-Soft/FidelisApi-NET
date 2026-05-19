using Fidelis.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fidelis.Infrastructure.Persistence.Configurations;

public class VeterinarioConfiguration : IEntityTypeConfiguration<Veterinario>
{
    public void Configure(EntityTypeBuilder<Veterinario> builder)
    {
        builder.ToTable("VETERINARIOS");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Crmv)
            .IsRequired()
            .HasMaxLength(13);

        builder.Property(x => x.Nome)
            .IsRequired()
            .HasMaxLength(75);

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(75);

        builder.Property(x => x.Senha)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Especialidade)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.DataCriacao)
            .IsRequired();

        builder.HasOne(x => x.Clinica)
            .WithMany(x => x.Veterinarios)
            .HasForeignKey(x => x.ClinicaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Consultas)
            .WithOne(x => x.Veterinario)
            .HasForeignKey(x => x.VeterinarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Vacinacoes)
            .WithOne(x => x.Veterinario)
            .HasForeignKey(x => x.VeterinarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Vermifugacoes)
            .WithOne(x => x.Veterinario)
            .HasForeignKey(x => x.VeterinarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}