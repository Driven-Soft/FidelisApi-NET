namespace Fidelis.Application.DTOs;

public class AnalisePesoRequest
{
    public int PetId { get; set; }
    public string Especie { get; set; } = string.Empty;
    public string? Raca { get; set; }
    public string Sexo { get; set; } = string.Empty;
    public double IdadeAnos { get; set; }
    public decimal PesoKg { get; set; }
}

public class AnalisePesoResponse
{
    public int PetId { get; set; }
    public PerfilAnalisado Perfil { get; set; } = new();
    public AvaliacaoPeso Avaliacao { get; set; } = new();
}

public class PerfilAnalisado
{
    public string Especie { get; set; } = string.Empty;
    public string? Raca { get; set; }
    public string Sexo { get; set; } = string.Empty;
    public double IdadeAnos { get; set; }
    public bool RacaNaBaseDeReferencia { get; set; }
}

public class AvaliacaoPeso
{
    public string Risco { get; set; } = string.Empty;
    public string TipoRecomendacao { get; set; } = string.Empty;
    public FaixaPeso FaixaEsperadaKg { get; set; } = new();
    public decimal PesoInformadoKg { get; set; }
    public double DesvioAlemDaFaixa { get; set; }
    public string Justificativa { get; set; } = string.Empty;
    public List<AcaoSugerida> Acoes { get; set; } = [];
}

public class FaixaPeso
{
    public decimal Min { get; set; }
    public decimal Max { get; set; }
}

public class AcaoSugerida
{
    public string Registro { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public int? PrazoDias { get; set; }
}

public class AvaliacaoPesoResponse
{
    public int PetId { get; }
    public int HistoricoPesoId { get; }
    public DateTime DataMedicao { get; }
    public PerfilAnalisado Perfil { get; }
    public AvaliacaoPeso Avaliacao { get; }

    public AvaliacaoPesoResponse(int historicoPesoId, DateTime dataMedicao, AnalisePesoResponse analise)
    {
        PetId = analise.PetId;
        HistoricoPesoId = historicoPesoId;
        DataMedicao = dataMedicao;
        Perfil = analise.Perfil;
        Avaliacao = analise.Avaliacao;
    }
}
