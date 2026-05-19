using Fidelis.Application.Interfaces.Repositories;
using Fidelis.Domain.Entities;
using Fidelis.Infrastructure.Persistence;

namespace Fidelis.Infrastructure.Repositories;

public class PetRepository : Repository<Pet>, IPetRepository
{
    public PetRepository(FidelisContext context) : base(context)
    {
    }
}