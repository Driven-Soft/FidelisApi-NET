using Fidelis.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fidelis.Infrastructure.Persistence;

public class FidelisContext : DbContext
{
    public FidelisContext(DbContextOptions<FidelisContext> options) : base(options)
    {
    }

    public DbSet<Clinica> Clinicas { get; set; }
    public DbSet<Comportamento> Comportamentos { get; set; }
    public DbSet<Consulta> Consultas { get; set; }
    public DbSet<Exame> Exames { get; set; }
    public DbSet<HistoricoPeso> HistoricoPesos { get; set; }
    public DbSet<Lembrete> Lembretes { get; set; }
    public DbSet<Medicamento> Medicamentos { get; set; }
    public DbSet<Pet> Pets { get; set; }
    public DbSet<Prescricao> Prescricoes { get; set; }
    public DbSet<Recomendacao> Recomendacoes { get; set; }
    public DbSet<Tutor> Tutores { get; set; }
    public DbSet<Vacinacao> Vacinacoes { get; set; }
    public DbSet<Vermifugacao> Vermifugacoes { get; set; }
    public DbSet<Veterinario> Veterinarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FidelisContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}