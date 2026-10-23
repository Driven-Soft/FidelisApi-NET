namespace Fidelis.Infrastructure.Mongo;

public class MongoSettings
{
    public const string SectionName = "MongoDb";

    public string ConnectionString { get; set; } = string.Empty;

    public string DatabaseName { get; set; } = "fidelis";

    public int ServerSelectionTimeoutSeconds { get; set; } = 5;
}
