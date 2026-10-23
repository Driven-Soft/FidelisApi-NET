using Fidelis.Application.Common;
using Fidelis.Application.DTOs;

namespace Fidelis.Application.Services.Interfaces;

public interface IExameService
{
    PagedResult<ExameResponse> GetAll(ExameQueryParameters query);
    ExameResponse? GetById(int id);
    ExameResponse Create(ExameRequest request);
    ExameResponse? Update(int id, ExameRequest request);
    ExameResponse? Patch(int id, ExamePatchRequest request);
    bool Delete(int id);
}
