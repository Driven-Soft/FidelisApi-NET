using Fidelis.Application.Common;
using Fidelis.Application.DTOs;

namespace Fidelis.Application.Services.Interfaces;

public interface IClinicaService
{
    PagedResult<ClinicaResponse> GetAll(ClinicaQueryParameters query);
    ClinicaResponse? GetById(int id);
    ClinicaResponse Create(ClinicaRequest request);
    ClinicaResponse? Update(int id, ClinicaRequest request);
    ClinicaResponse? Patch(int id, ClinicaPatchRequest request);
    bool Delete(int id);
}
