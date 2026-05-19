using Fidelis.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fidelis.Infrastructure.Persistence.Configurations;

public class ClinicaConfiguration : IEntityTypeConfiguration<Clinica>
{
    public void Configure(EntityTypeBuilder<Clinica> builder)
    {
        builder.ToTable("CLINICAS");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Nome)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Cnpj)
            .IsRequired()
            .HasMaxLength(18);

        builder.Property(x => x.Telefone)
            .IsRequired()
            .HasMaxLength(15);

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(75);

        builder.Property(x => x.Endereco)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasMany(x => x.Veterinarios)
            .WithOne(x => x.Clinica)
            .HasForeignKey(x => x.ClinicaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Pets)
            .WithOne(x => x.Clinica)
            .HasForeignKey(x => x.ClinicaId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

