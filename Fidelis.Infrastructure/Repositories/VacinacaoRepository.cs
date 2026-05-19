using Fidelis.Application.Interfaces.Repositories;
using Fidelis.Domain.Entities;
using Fidelis.Infrastructure.Persistence;

namespace Fidelis.Infrastructure.Repositories;

public class VacinacaoRepository : Repository<Vacinacao>, IVacinacaoRepository
{
    public VacinacaoRepository(FidelisContext context) : base(context)
    {
    }
}
