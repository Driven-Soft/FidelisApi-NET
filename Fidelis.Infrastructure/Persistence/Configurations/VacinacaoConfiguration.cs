using Fidelis.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fidelis.Infrastructure.Persistence.Configurations;

public class VacinacaoConfiguration : IEntityTypeConfiguration<Vacinacao>
{
    public void Configure(EntityTypeBuilder<Vacinacao> builder)
    {
        builder.ToTable("VACINACOES");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.DataAplicacao)
            .IsRequired();

        builder.Property(x => x.DataProxima)
            .IsRequired();

        builder.Property(x => x.VacinaAplicada)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Observacao);

        builder.HasOne(x => x.Pet)
            .WithMany(x => x.Vacinacoes)
            .HasForeignKey(x => x.PetId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Veterinario)
            .WithMany(x => x.Vacinacoes)
            .HasForeignKey(x => x.VeterinarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

