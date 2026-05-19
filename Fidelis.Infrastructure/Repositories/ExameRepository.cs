using Fidelis.Application.Interfaces.Repositories;
using Fidelis.Domain.Entities;
using Fidelis.Infrastructure.Persistence;

namespace Fidelis.Infrastructure.Repositories;

public class ExameRepository : Repository<Exame>, IExameRepository
{
    public ExameRepository(FidelisContext context) : base(context)
    {
    }
}
