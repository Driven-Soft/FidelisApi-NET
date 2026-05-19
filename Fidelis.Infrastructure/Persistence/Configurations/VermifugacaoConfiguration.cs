using Fidelis.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fidelis.Infrastructure.Persistence.Configurations;

public class VermifugacaoConfiguration : IEntityTypeConfiguration<Vermifugacao>
{
    public void Configure(EntityTypeBuilder<Vermifugacao> builder)
    {
        builder.ToTable("VERMIFUGACOES");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Produto)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.DataAplicacao)
            .IsRequired();

        builder.Property(x => x.DataProxima)
            .IsRequired();

        builder.HasOne(x => x.Pet)
            .WithMany(x => x.Vermifugacoes)
            .HasForeignKey(x => x.PetId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Veterinario)
            .WithMany(x => x.Vermifugacoes)
            .HasForeignKey(x => x.VeterinarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

