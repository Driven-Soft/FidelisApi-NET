using Fidelis.Application.Interfaces.Repositories;
using Fidelis.Domain.Entities;
using Fidelis.Infrastructure.Persistence;

namespace Fidelis.Infrastructure.Repositories;

public class TutorRepository : Repository<Tutor>, ITutorRepository
{
    public TutorRepository(FidelisContext context) : base(context)
    {
    }

    public Tutor? GetByEmail(string email)
    {
        var normalized = email.Trim().ToUpper();

        return Set
            .Where(tutor => tutor.Email.ToUpper() == normalized)
            .OrderByDescending(tutor => tutor.DataCriacao)
            .FirstOrDefault();
    }

    public bool ExistsByEmail(string email)
    {
        return GetByEmail(email) is not null;
    }
}
