using MySql.Data.MySqlClient;

namespace GestionScolaire.Core.Data;

public static class Db
{
    // Variables d'environnement recommandées :
    // GS_DB_SERVER, GS_DB_PORT, GS_DB_NAME, GS_DB_USER, GS_DB_PASSWORD
    private static string BuildConnectionString()
    {
        var b = new MySqlConnectionStringBuilder
        {
            Server = Environment.GetEnvironmentVariable("GS_DB_SERVER") ?? "localhost",
            Port = uint.TryParse(Environment.GetEnvironmentVariable("GS_DB_PORT"), out var p) ? p : 3306,
            Database = Environment.GetEnvironmentVariable("GS_DB_NAME") ?? "gestion_scolaire",
            UserID = Environment.GetEnvironmentVariable("GS_DB_USER") ?? "app_scolaire",
            Password = Environment.GetEnvironmentVariable("GS_DB_PASSWORD") ?? "CHANGE_ME_Strong#2026",
            CharacterSet = "utf8mb4",
            Pooling = true,
            ConnectionTimeout = 10,
            DefaultCommandTimeout = 30
        };
        return b.ConnectionString;
    }

    private static readonly string ConnectionString = BuildConnectionString();
    public static MySqlConnection CreateConnection() => new(ConnectionString);
    public static string SafeDescription =>
        $"Serveur={Environment.GetEnvironmentVariable("GS_DB_SERVER") ?? "localhost"}; Base=gestion_scolaire";
}
