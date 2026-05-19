using Fidelis.Application.Interfaces.Repositories;
using Fidelis.Domain.Entities;
using Fidelis.Infrastructure.Persistence;

namespace Fidelis.Infrastructure.Repositories;

public class MedicamentoRepository : Repository<Medicamento>, IMedicamentoRepository
{
    public MedicamentoRepository(FidelisContext context) : base(context)
    {
    }
}
