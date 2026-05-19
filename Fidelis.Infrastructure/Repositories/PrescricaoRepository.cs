using Fidelis.Application.Interfaces.Repositories;
using Fidelis.Domain.Entities;
using Fidelis.Infrastructure.Persistence;

namespace Fidelis.Infrastructure.Repositories;

public class PrescricaoRepository : Repository<Prescricao>, IPrescricaoRepository
{
    public PrescricaoRepository(FidelisContext context) : base(context)
    {
    }
}
