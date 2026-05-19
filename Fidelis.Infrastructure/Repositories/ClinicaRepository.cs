using Fidelis.Application.Interfaces.Repositories;
using Fidelis.Domain.Entities;
using Fidelis.Infrastructure.Persistence;

namespace Fidelis.Infrastructure.Repositories;

public class ClinicaRepository : Repository<Clinica>, IClinicaRepository
{
    public ClinicaRepository(FidelisContext context) : base(context)
    {
    }
}
