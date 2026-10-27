using System.Globalization;
using System.Text;
using Fidelis.Application.DTOs;
using Fidelis.Application.Interfaces.Clients;
using Fidelis.Application.Interfaces.Repositories;
using Fidelis.Application.Services.Interfaces;
using Fidelis.Application.Settings;
using Fidelis.Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Fidelis.Application.Services.Implementations;

public class AnalisePesoService : IAnalisePesoService
{
    private static readonly Dictionary<string, string> Especies = new()
    {
        ["cachorro"] = "Cachorro", ["cachorra"] = "Cachorro", ["cao"] = "Cachorro", ["caes"] = "Cachorro",
        ["cadela"] = "Cachorro", ["canino"] = "Cachorro", ["canina"] = "Cachorro", ["dog"] = "Cachorro",
        ["gato"] = "Gato", ["gata"] = "Gato", ["felino"] = "Gato", ["felina"] = "Gato", ["cat"] = "Gato"
    };

    private readonly IPetRepository _petRepository;
    private readonly IHistoricoPesoRepository _historicoPesoRepository;
    private readonly IRecomendacaoRepository _recomendacaoRepository;
    private readonly ILembreteRepository _lembreteRepository;
    private readonly IFidelisAiClient _client;
    private readonly FidelisAiSettings _settings;
    private readonly ILogger<AnalisePesoService> _logger;

    public AnalisePesoService(
        IPetRepository petRepository,
        IHistoricoPesoRepository historicoPesoRepository,
        IRecomendacaoRepository recomendacaoRepository,
        ILembreteRepository lembreteRepository,
        IFidelisAiClient client,
        IOptions<FidelisAiSettings> settings,
        ILogger<AnalisePesoService> logger)
    {
        _petRepository = petRepository;
        _historicoPesoRepository = historicoPesoRepository;
        _recomendacaoRepository = recomendacaoRepository;
        _lembreteRepository = lembreteRepository;
        _client = client;
        _settings = settings.Value;
        _logger = logger;
    }

    public AvaliacaoPeso? AnalisarPesagem(HistoricoPeso pesagem)
    {
        if (string.IsNullOrWhiteSpace(_settings.BaseUrl))
            return null;

        try
        {
            var ultima = _historicoPesoRepository.GetLatestByPetId(pesagem.PetId);
            if (ultima is not null && ultima.DataMedicao > pesagem.DataMedicao)
            {
                _logger.LogInformation("Pesagem {HistoricoPesoId} do pet {PetId} é retroativa e não foi analisada", pesagem.Id, pesagem.PetId);
                return null;
            }

            var pet = _petRepository.GetById(pesagem.PetId);
            if (pet is null)
                return null;

            var especie = NormalizarEspecie(pet.Especie);
            if (especie is null)
            {
                _logger.LogInformation("Espécie {Especie} do pet {PetId} não é avaliada pelo serviço de IA", pet.Especie, pesagem.PetId);
                return null;
            }

            var analise = _client.Analisar(NovaAnalise(pet, especie, pesagem));
            GravarAcoes(pet, pesagem.PetId, analise.Avaliacao);
            return analise.Avaliacao;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Análise de peso falhou para a pesagem {HistoricoPesoId} do pet {PetId}", pesagem.Id, pesagem.PetId);
            return null;
        }
    }

    public AvaliacaoPesoResponse? GetAvaliacao(int petId)
    {
        var pet = _petRepository.GetById(petId);
        var pesagem = _historicoPesoRepository.GetLatestByPetId(petId);
        if (pet is null || pesagem is null)
            return null;

        var especie = NormalizarEspecie(pet.Especie)
            ?? throw new InvalidOperationException("A avaliação de peso está disponível apenas para cães e gatos.");

        if (string.IsNullOrWhiteSpace(_settings.BaseUrl))
            throw new InvalidOperationException("O serviço de IA não está configurado.");

        var analise = _client.Analisar(NovaAnalise(pet, especie, pesagem));
        return new AvaliacaoPesoResponse(pesagem.Id, pesagem.DataMedicao, analise);
    }

    private void GravarAcoes(Pet pet, int petId, AvaliacaoPeso avaliacao)
    {
        if (avaliacao.Acoes.Count == 0)
            return;

        var agora = DateTime.UtcNow;
        if (_recomendacaoRepository.ExistsByPetIdAndTipo(petId, avaliacao.TipoRecomendacao, agora.AddDays(-_settings.JanelaRecomendacaoDias)))
        {
            _logger.LogInformation("Pet {PetId} já recebeu {TipoRecomendacao} nos últimos {Dias} dias", petId, avaliacao.TipoRecomendacao, _settings.JanelaRecomendacaoDias);
            return;
        }

        foreach (var acao in avaliacao.Acoes)
        {
            if (acao.Registro == "RECOMENDACAO")
                _recomendacaoRepository.Add(new Recomendacao(acao.Tipo, acao.Descricao, agora, petId));
            else if (acao.Registro == "LEMBRETE")
                _lembreteRepository.Add(new Lembrete(acao.Tipo, acao.Descricao, agora.Date.AddDays(acao.PrazoDias ?? 0), pet.TutorId, petId));
        }

        _recomendacaoRepository.SaveChanges();
    }

    private static AnalisePesoRequest NovaAnalise(Pet pet, string especie, HistoricoPeso pesagem) => new()
    {
        PetId = pesagem.PetId,
        Especie = especie,
        Raca = pet.Raca,
        Sexo = pet.Sexo.ToString(),
        IdadeAnos = CalcularIdadeAnos(pet.DataNascimento, pesagem.DataMedicao),
        PesoKg = pesagem.PesoKg
    };

    private static string? NormalizarEspecie(string especie)
    {
        var semAcento = new string(especie.Trim().ToLowerInvariant()
            .Normalize(NormalizationForm.FormD)
            .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            .ToArray());

        return Especies.GetValueOrDefault(semAcento);
    }

    private static double CalcularIdadeAnos(DateTime dataNascimento, DateTime dataMedicao)
    {
        var dias = (dataMedicao.Date - dataNascimento.Date).TotalDays;
        return Math.Round(Math.Max(dias, 0) / 365.25, 2);
    }
}
