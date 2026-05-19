using Fidelis.Application.Interfaces.Repositories;
using Fidelis.Domain.Entities;
using Fidelis.Infrastructure.Persistence;

namespace Fidelis.Infrastructure.Repositories;

public class VermifugacaoRepository : Repository<Vermifugacao>, IVermifugacaoRepository
{
    public VermifugacaoRepository(FidelisContext context) : base(context)
    {
    }
}
