using Fidelis.Application.Interfaces.Repositories;
using Fidelis.Domain.Entities;
using Fidelis.Infrastructure.Persistence;

namespace Fidelis.Infrastructure.Repositories;

public class RecomendacaoRepository : Repository<Recomendacao>, IRecomendacaoRepository
{
    public RecomendacaoRepository(FidelisContext context) : base(context)
    {
    }
}
