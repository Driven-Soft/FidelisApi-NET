using Fidelis.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fidelis.Infrastructure.Persistence.Configurations;

public class LembreteConfiguration : IEntityTypeConfiguration<Lembrete>
{
    public void Configure(EntityTypeBuilder<Lembrete> builder)
    {
        builder.ToTable("Lembretes");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Tipo)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Descricao)
            .IsRequired();

        builder.Property(x => x.DataPrevista)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired();

        builder.HasOne(x => x.Tutor)
            .WithMany(x => x.Lembretes)
            .HasForeignKey(x => x.TutorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Pet)
            .WithMany(x => x.Lembretes)
            .HasForeignKey(x => x.PetId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

