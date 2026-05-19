using Fidelis.Application.Interfaces.Repositories;
using Fidelis.Domain.Entities;
using Fidelis.Infrastructure.Persistence;

namespace Fidelis.Infrastructure.Repositories;

public class ConsultaRepository : Repository<Consulta>, IConsultaRepository
{
    public ConsultaRepository(FidelisContext context) : base(context)
    {
    }
}
