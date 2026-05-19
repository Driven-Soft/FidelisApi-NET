using Fidelis.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fidelis.Infrastructure.Persistence.Configurations;

public class ComportamentoConfiguration : IEntityTypeConfiguration<Comportamento>
{
    public void Configure(EntityTypeBuilder<Comportamento> builder)
    {
        builder.ToTable("COMPORTAMENTOS");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Data)
            .IsRequired();

        builder.Property(x => x.Descricao)
            .IsRequired();

        builder.HasOne(x => x.Pet)
            .WithMany(x => x.Comportamentos)
            .HasForeignKey(x => x.PetId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

