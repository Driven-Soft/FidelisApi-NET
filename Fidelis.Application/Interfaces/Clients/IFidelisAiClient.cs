using Fidelis.Application.DTOs;

namespace Fidelis.Application.Interfaces.Clients;

public interface IFidelisAiClient
{
    AnalisePesoResponse Analisar(AnalisePesoRequest request);
}
