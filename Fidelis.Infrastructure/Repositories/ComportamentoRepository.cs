using Fidelis.Application.Interfaces.Repositories;
using Fidelis.Domain.Entities;
using Fidelis.Infrastructure.Persistence;

namespace Fidelis.Infrastructure.Repositories;

public class ComportamentoRepository : Repository<Comportamento>, IComportamentoRepository
{
    public ComportamentoRepository(FidelisContext context) : base(context)
    {
    }
}
