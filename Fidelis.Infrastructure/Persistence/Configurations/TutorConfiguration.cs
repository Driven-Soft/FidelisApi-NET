using Fidelis.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fidelis.Infrastructure.Persistence.Configurations;

public class TutorConfiguration : IEntityTypeConfiguration<Tutor>
{
    public void Configure(EntityTypeBuilder<Tutor> builder)
    {
        builder.ToTable("TUTORES");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Cpf)
            .IsRequired()
            .HasMaxLength(14);

        builder.Property(x => x.Nome)
            .IsRequired()
            .HasMaxLength(75);

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(75);

        builder.Property(x => x.Senha)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Telefone)
            .IsRequired()
            .HasMaxLength(15);

        builder.Property(x => x.Endereco)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.DataCriacao)
            .IsRequired();

        builder.HasMany(x => x.Pets)
            .WithOne(x => x.Tutor)
            .HasForeignKey(x => x.TutorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Lembretes)
            .WithOne(x => x.Tutor)
            .HasForeignKey(x => x.TutorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}