using Fidelis.Domain.Entities;

namespace Fidelis.Application.Interfaces.Repositories;

public interface IRecomendacaoRepository : IRepository<Recomendacao>
{
    bool ExistsByPetIdAndTipo(int petId, string tipo, DateTime desde);
}

