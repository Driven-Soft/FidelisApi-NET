using Fidelis.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fidelis.Infrastructure.Persistence.Configurations;

public class RecomendacaoConfiguration : IEntityTypeConfiguration<Recomendacao>
{
    public void Configure(EntityTypeBuilder<Recomendacao> builder)
    {
        builder.ToTable("RECOMENDACOES");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Tipo)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Descricao)
            .IsRequired();

        builder.Property(x => x.DataRecomendacao)
            .IsRequired();

        builder.HasOne(x => x.Pet)
            .WithMany(x => x.Recomendacoes)
            .HasForeignKey(x => x.PetId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

