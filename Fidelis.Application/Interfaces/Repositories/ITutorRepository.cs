using Fidelis.Domain.Entities;

namespace Fidelis.Application.Interfaces.Repositories;

public interface ITutorRepository : IRepository<Tutor>
{
    Tutor? GetByEmail(string email);
    bool ExistsByEmail(string email);
}
