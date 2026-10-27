using Fidelis.Application.Interfaces.Repositories;
using Fidelis.Domain.Entities;
using Fidelis.Infrastructure.Persistence;

namespace Fidelis.Infrastructure.Repositories;

public class VeterinarioRepository : Repository<Veterinario>, IVeterinarioRepository
{
    public VeterinarioRepository(FidelisContext context) : base(context)
    {
    }

    public Veterinario? GetByEmail(string email)
    {
        var normalized = email.Trim().ToUpper();

        return Set
            .Where(veterinario => veterinario.Email.ToUpper() == normalized)
            .OrderByDescending(veterinario => veterinario.DataCriacao)
            .FirstOrDefault();
    }
}
