using Fidelis.Application.DTOs;

namespace Fidelis.Application.Services.Interfaces;

public interface ITutorService
{
    List<TutorResponse> GetAll();
    TutorResponse? GetById(int id);
    TutorResponse Create(TutorRequest request);
    TutorResponse? Update(int id, TutorRequest request);
    TutorResponse? Patch(int id, TutorPatchRequest request);
    bool Delete(int id);
}