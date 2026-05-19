using Fidelis.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fidelis.Infrastructure.Persistence.Configurations;

public class HistoricoPesoConfiguration : IEntityTypeConfiguration<HistoricoPeso>
{
    public void Configure(EntityTypeBuilder<HistoricoPeso> builder)
    {
        builder.ToTable("HistoricoPesos");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.PesoKg)
            .IsRequired()
            .HasPrecision(7, 2);

        builder.Property(x => x.DataMedicao)
            .IsRequired();

        builder.Property(x => x.Observacao);

        builder.HasOne(x => x.Pet)
            .WithMany(x => x.HistoricoPesos)
            .HasForeignKey(x => x.PetId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

