using Fidelis.Application.DTOs;

namespace Fidelis.Application.Services.Interfaces;

public interface IClinicaService
{
    List<ClinicaResponse> GetAll();
    ClinicaResponse? GetById(int id);
    ClinicaResponse Create(ClinicaRequest request);
    ClinicaResponse? Update(int id, ClinicaRequest request);
    ClinicaResponse? Patch(int id, ClinicaPatchRequest request);
    bool Delete(int id);
}
