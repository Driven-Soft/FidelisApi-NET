using Fidelis.Application.Interfaces.Repositories;
using Fidelis.Domain.Entities;
using Fidelis.Infrastructure.Persistence;

namespace Fidelis.Infrastructure.Repositories;

public class TutorRepository : Repository<Tutor>, ITutorRepository
{
    public TutorRepository(FidelisContext context) : base(context)
    {
    }
}