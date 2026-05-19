using Fidelis.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fidelis.Infrastructure.Persistence.Configurations;

public class PetConfiguration : IEntityTypeConfiguration<Pet>
{
    public void Configure(EntityTypeBuilder<Pet> builder)
    {
        builder.ToTable("Pets");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Nome)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(x => x.Especie)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.Raca)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.Sexo)
            .IsRequired();

        builder.Property(x => x.DataNascimento)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.FotoUrl)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasOne(x => x.Tutor)
            .WithMany(x => x.Pets)
            .HasForeignKey(x => x.TutorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Clinica)
            .WithMany(x => x.Pets)
            .HasForeignKey(x => x.ClinicaId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(x => x.Consultas)
            .WithOne(x => x.Pet)
            .HasForeignKey(x => x.PetId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Vacinacoes)
            .WithOne(x => x.Pet)
            .HasForeignKey(x => x.PetId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Vermifugacoes)
            .WithOne(x => x.Pet)
            .HasForeignKey(x => x.PetId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.HistoricoPesos)
            .WithOne(x => x.Pet)
            .HasForeignKey(x => x.PetId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Comportamentos)
            .WithOne(x => x.Pet)
            .HasForeignKey(x => x.PetId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Recomendacoes)
            .WithOne(x => x.Pet)
            .HasForeignKey(x => x.PetId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Lembretes)
            .WithOne(x => x.Pet)
            .HasForeignKey(x => x.PetId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}