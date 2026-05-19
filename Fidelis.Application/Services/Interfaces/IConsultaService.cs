using Fidelis.Application.DTOs;

namespace Fidelis.Application.Services.Interfaces;

public interface IConsultaService
{
    List<ConsultaResponse> GetAll();
    ConsultaResponse? GetById(int id);
    ConsultaResponse Create(ConsultaRequest request);
    ConsultaResponse? Update(int id, ConsultaRequest request);
    ConsultaResponse? Patch(int id, ConsultaPatchRequest request);
    bool Delete(int id);
}
