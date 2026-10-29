using Fidelis.Application.DTOs;
using Fidelis.Application.Interfaces.Clients;

namespace Fidelis.IntegrationTests.Fakes;

public class InMemoryFidelisAiClient : IFidelisAiClient
{
    private string _risco = "VERDE";
    private bool _indisponivel;

    public void ResponderCom(string risco)
    {
        _risco = risco;
        _indisponivel = false;
    }

    public void Falhar() => _indisponivel = true;

    public AnalisePesoResponse Analisar(AnalisePesoRequest request)
    {
        if (_indisponivel)
            throw new HttpRequestException("Serviço de IA fora do ar");

        var (tipo, acoes) = _risco switch
        {
            "AMARELO" => ("PESO_ATENCAO", new List<AcaoSugerida>
            {
                new() { Registro = "RECOMENDACAO", Tipo = "PESO_ATENCAO", Descricao = "Vale revisar a alimentação." },
                new() { Registro = "LEMBRETE", Tipo = "PESAGEM", Descricao = "Registrar nova pesagem.", PrazoDias = 15 }
            }),
            "VERMELHO" => ("PESO_ALERTA", new List<AcaoSugerida>
            {
                new() { Registro = "RECOMENDACAO", Tipo = "PESO_ALERTA", Descricao = "Agende uma consulta." },
                new() { Registro = "LEMBRETE", Tipo = "CONSULTA", Descricao = "Agendar consulta.", PrazoDias = 7 }
            }),
            _ => ("PESO_ADEQUADO", new List<AcaoSugerida>())
        };

        return new AnalisePesoResponse
        {
            PetId = request.PetId,
            Perfil = new PerfilAnalisado { Especie = request.Especie, Raca = request.Raca, Sexo = request.Sexo, IdadeAnos = request.IdadeAnos },
            Avaliacao = new AvaliacaoPeso
            {
                Risco = _risco,
                TipoRecomendacao = tipo,
                FaixaEsperadaKg = new FaixaPeso { Min = 2.1m, Max = 2.7m },
                PesoInformadoKg = request.PesoKg,
                Justificativa = $"Avaliação {_risco} de teste.",
                Acoes = acoes
            }
        };
    }
}
