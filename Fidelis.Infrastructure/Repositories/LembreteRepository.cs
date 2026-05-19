using Fidelis.Application.Interfaces.Repositories;
using Fidelis.Domain.Entities;
using Fidelis.Infrastructure.Persistence;

namespace Fidelis.Infrastructure.Repositories;

public class LembreteRepository : Repository<Lembrete>, ILembreteRepository
{
    public LembreteRepository(FidelisContext context) : base(context)
    {
    }
}
