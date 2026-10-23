namespace Fidelis.Domain.Entities;

public class RegistroDiario
{
    public const int MaxTags = 10;
    public const int MaxTituloLength = 100;
    public const int MaxDescricaoLength = 2000;

    public static readonly IReadOnlyList<string> HumoresPermitidos =
        ["Feliz", "Calmo", "Agitado", "Ansioso", "Triste", "Doente"];

    public string Id { get; private set; } = string.Empty;

    public int PetId { get; private set; }

    public string Titulo { get; private set; } = string.Empty;

    public string Descricao { get; private set; } = string.Empty;

    public string Humor { get; private set; } = string.Empty;

    public List<string> Tags { get; private set; } = [];

    public DateTime DataRegistro { get; private set; }

    public DateTime CriadoEm { get; private set; }

    public DateTime? AtualizadoEm { get; private set; }

    public RegistroDiario(int petId, string titulo, string descricao, string humor, IEnumerable<string>? tags, DateTime dataRegistro)
    {
        if (petId <= 0)
            throw new ArgumentException("Pet é obrigatório");

        PetId = petId;
        ApplyTitulo(titulo);
        ApplyDescricao(descricao);
        ApplyHumor(humor);
        ApplyTags(tags);
        ApplyDataRegistro(dataRegistro);
        CriadoEm = DateTime.UtcNow;
    }

    public void UpdateTitulo(string titulo)
    {
        ApplyTitulo(titulo);
        Touch();
    }

    public void UpdateDescricao(string descricao)
    {
        ApplyDescricao(descricao);
        Touch();
    }

    public void UpdateHumor(string humor)
    {
        ApplyHumor(humor);
        Touch();
    }

    public void UpdateTags(IEnumerable<string>? tags)
    {
        ApplyTags(tags);
        Touch();
    }

    public void UpdateDataRegistro(DateTime dataRegistro)
    {
        ApplyDataRegistro(dataRegistro);
        Touch();
    }

    public void Update(string titulo, string descricao, string humor, IEnumerable<string>? tags, DateTime dataRegistro)
    {
        ApplyTitulo(titulo);
        ApplyDescricao(descricao);
        ApplyHumor(humor);
        ApplyTags(tags);
        ApplyDataRegistro(dataRegistro);
        Touch();
    }

    private void ApplyTitulo(string titulo)
    {
        if (string.IsNullOrWhiteSpace(titulo))
            throw new ArgumentException("Título é obrigatório");
        if (titulo.Trim().Length > MaxTituloLength)
            throw new ArgumentException($"Título deve ter no máximo {MaxTituloLength} caracteres");
        Titulo = titulo.Trim();
    }

    private void ApplyDescricao(string descricao)
    {
        if (string.IsNullOrWhiteSpace(descricao))
            throw new ArgumentException("Descrição é obrigatória");
        if (descricao.Trim().Length > MaxDescricaoLength)
            throw new ArgumentException($"Descrição deve ter no máximo {MaxDescricaoLength} caracteres");
        Descricao = descricao.Trim();
    }

    private void ApplyHumor(string humor)
    {
        var humorValido = HumoresPermitidos.FirstOrDefault(h => h.Equals(humor?.Trim(), StringComparison.OrdinalIgnoreCase));
        Humor = humorValido ?? throw new ArgumentException($"Humor deve ser um destes valores: {string.Join(", ", HumoresPermitidos)}");
    }

    private void ApplyTags(IEnumerable<string>? tags)
    {
        var normalizadas = (tags ?? [])
            .Where(tag => !string.IsNullOrWhiteSpace(tag))
            .Select(tag => tag.Trim().ToLowerInvariant())
            .Distinct()
            .ToList();

        if (normalizadas.Count > MaxTags)
            throw new ArgumentException($"São permitidas no máximo {MaxTags} tags");

        Tags = normalizadas;
    }

    private void ApplyDataRegistro(DateTime dataRegistro)
    {
        if (dataRegistro == default)
            throw new ArgumentException("Data do registro é obrigatória");
        if (dataRegistro > DateTime.UtcNow.AddDays(1))
            throw new ArgumentException("Data do registro não pode estar no futuro");
        DataRegistro = dataRegistro;
    }

    private void Touch() => AtualizadoEm = DateTime.UtcNow;

    public override string ToString() => $"Registro de diário: {Titulo} - {DataRegistro:dd/MM/yyyy} ({Humor})";

    public RegistroDiario() { }
}
