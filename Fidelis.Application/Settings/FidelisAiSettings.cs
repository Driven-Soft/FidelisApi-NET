namespace Fidelis.Application.Settings;

public class FidelisAiSettings
{
    public const string SectionName = "FidelisAi";

    public string BaseUrl { get; set; } = string.Empty;

    public string ApiKey { get; set; } = string.Empty;

    public int TimeoutSeconds { get; set; } = 3;

    public int JanelaRecomendacaoDias { get; set; } = 30;
}
