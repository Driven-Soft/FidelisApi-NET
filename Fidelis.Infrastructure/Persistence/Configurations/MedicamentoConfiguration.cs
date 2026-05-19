using Fidelis.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fidelis.Infrastructure.Persistence.Configurations;

public class MedicamentoConfiguration : IEntityTypeConfiguration<Medicamento>
{
    public void Configure(EntityTypeBuilder<Medicamento> builder)
    {
        builder.ToTable("Medicamentos");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Nome)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Descricao)
            .IsRequired();

        builder.HasOne(x => x.Prescricao)
            .WithMany(x => x.Medicamentos)
            .HasForeignKey(x => x.PrescricaoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

