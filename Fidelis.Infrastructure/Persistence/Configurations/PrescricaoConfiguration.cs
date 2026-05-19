using Fidelis.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fidelis.Infrastructure.Persistence.Configurations;

public class PrescricaoConfiguration : IEntityTypeConfiguration<Prescricao>
{
    public void Configure(EntityTypeBuilder<Prescricao> builder)
    {
        builder.ToTable("PRESCRICOES");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Dosagem)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Frequencia)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.DuracaoDias)
            .IsRequired();

        builder.Property(x => x.Observacao);

        builder.HasOne(x => x.Consulta)
            .WithMany(x => x.Prescricoes)
            .HasForeignKey(x => x.ConsultaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Medicamentos)
            .WithOne(x => x.Prescricao)
            .HasForeignKey(x => x.PrescricaoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
