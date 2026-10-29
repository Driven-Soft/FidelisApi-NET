namespace Fidelis.IntegrationTests.Dtos;

public class LinkDto
{
    public string Href { get; set; } = string.Empty;
    public string Rel { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
}

public class ResourceDto<T>
{
    public T Data { get; set; } = default!;
    public List<LinkDto> Links { get; set; } = [];
}

public class PagedDto<T>
{
    public List<ResourceDto<T>> Items { get; set; } = [];
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public List<LinkDto> Links { get; set; } = [];
}

public class PetResponseDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Especie { get; set; } = string.Empty;
    public string Raca { get; set; } = string.Empty;
    public char Sexo { get; set; }
    public DateTime DataNascimento { get; set; }
    public char Status { get; set; }
    public string FotoUrl { get; set; } = string.Empty;
    public int TutorId { get; set; }
    public int? ClinicaId { get; set; }
}

public class TutorResponseDto
{
    public int Id { get; set; }
    public string Cpf { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; }
}

public class RegistroDiarioResponseDto
{
    public string Id { get; set; } = string.Empty;
    public int PetId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string Humor { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = [];
    public DateTime DataRegistro { get; set; }
    public DateTime? AtualizadoEm { get; set; }
}

public class AuditoriaResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string Metodo { get; set; } = string.Empty;
    public string Caminho { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public bool Sucesso { get; set; }
    public string? UsuarioId { get; set; }
}

public class HistoricoPesoResponseDto
{
    public int Id { get; set; }
    public decimal PesoKg { get; set; }
    public DateTime DataMedicao { get; set; }
    public int PetId { get; set; }
    public AvaliacaoIaDto? AvaliacaoIa { get; set; }
}

public class AvaliacaoPesoResponseDto
{
    public int PetId { get; set; }
    public int HistoricoPesoId { get; set; }
    public DateTime DataMedicao { get; set; }
    public AvaliacaoIaDto Avaliacao { get; set; } = new();
}

public class AvaliacaoIaDto
{
    public string Risco { get; set; } = string.Empty;
    public string TipoRecomendacao { get; set; } = string.Empty;
    public FaixaPesoDto FaixaEsperadaKg { get; set; } = new();
    public decimal PesoInformadoKg { get; set; }
    public string Justificativa { get; set; } = string.Empty;
}

public class FaixaPesoDto
{
    public decimal Min { get; set; }
    public decimal Max { get; set; }
}
