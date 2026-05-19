using Fidelis.Application.Interfaces.Repositories;
using Fidelis.Domain.Entities;
using Fidelis.Infrastructure.Persistence;

namespace Fidelis.Infrastructure.Repositories;

public class HistoricoPesoRepository : Repository<HistoricoPeso>, IHistoricoPesoRepository
{
    public HistoricoPesoRepository(FidelisContext context) : base(context)
    {
    }
}
