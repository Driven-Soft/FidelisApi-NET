using Fidelis.Application.Common;
using Fidelis.Application.DTOs;

namespace Fidelis.Application.Services.Interfaces;

public interface ILembreteService
{
    PagedResult<LembreteResponse> GetAll(LembreteQueryParameters query);
    LembreteResponse? GetById(int id);
    LembreteResponse Create(LembreteRequest request);
    LembreteResponse? Update(int id, LembreteRequest request);
    LembreteResponse? Patch(int id, LembretePatchRequest request);
    bool Delete(int id);
}
