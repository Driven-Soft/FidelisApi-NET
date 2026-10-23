using Fidelis.Application.Common;
using Fidelis.Application.DTOs;

namespace Fidelis.Application.Services.Interfaces;

public interface IConsultaService
{
    PagedResult<ConsultaResponse> GetAll(ConsultaQueryParameters query);
    ConsultaResponse? GetById(int id);
    ConsultaResponse Create(ConsultaRequest request);
    ConsultaResponse? Update(int id, ConsultaRequest request);
    ConsultaResponse? Patch(int id, ConsultaPatchRequest request);
    bool Delete(int id);
}
