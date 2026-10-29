using Fidelis.Domain.Entities;

namespace Fidelis.Application.Interfaces.Repositories;

public interface IVeterinarioRepository : IRepository<Veterinario>
{
    Veterinario? GetByEmail(string email);
}
