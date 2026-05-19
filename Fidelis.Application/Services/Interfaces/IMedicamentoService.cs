using Fidelis.Application.DTOs;

namespace Fidelis.Application.Services.Interfaces;

public interface IMedicamentoService
{
    List<MedicamentoResponse> GetAll();
    MedicamentoResponse? GetById(int id);
    MedicamentoResponse Create(MedicamentoRequest request);
    MedicamentoResponse? Update(int id, MedicamentoRequest request);
    MedicamentoResponse? Patch(int id, MedicamentoPatchRequest request);
    bool Delete(int id);
}
