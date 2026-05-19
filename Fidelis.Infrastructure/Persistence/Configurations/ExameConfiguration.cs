using Fidelis.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fidelis.Infrastructure.Persistence.Configurations;

public class ExameConfiguration : IEntityTypeConfiguration<Exame>
{
    public void Configure(EntityTypeBuilder<Exame> builder)
    {
        builder.ToTable("Exames");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Tipo)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Descricao)
            .IsRequired();

        builder.Property(x => x.Resultado);

        builder.Property(x => x.Data)
            .IsRequired();

        builder.HasOne(x => x.Consulta)
            .WithMany(x => x.Exames)
            .HasForeignKey(x => x.ConsultaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

