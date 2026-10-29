using Fidelis.Application.Interfaces.Repositories;
using Fidelis.Domain.Entities;
using Fidelis.Infrastructure.Persistence;

namespace Fidelis.Infrastructure.Repositories;

public class HistoricoPesoRepository : Repository<HistoricoPeso>, IHistoricoPesoRepository
{
    public HistoricoPesoRepository(FidelisContext context) : base(context)
    {
    }

    public HistoricoPeso? GetLatestByPetId(int petId)
    {
        return Set
            .Where(historico => historico.PetId == petId)
            .OrderByDescending(historico => historico.DataMedicao)
            .ThenByDescending(historico => historico.Id)
            .FirstOrDefault();
    }
}
