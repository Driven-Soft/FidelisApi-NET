namespace Fidelis.IntegrationTests.Dtos;

// As classes *Response da Application (ex: PetResponse, TutorResponse) só
// possuem um construtor que recebe a entidade de domínio, então não podem ser
// desserializadas diretamente pelo System.Text.Json do lado do cliente HTTP.
// Estas classes espelham o formato JSON de saída apenas para uso nos testes.

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
