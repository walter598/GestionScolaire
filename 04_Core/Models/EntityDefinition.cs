namespace GestionScolaire.Core.Models;

public sealed record FieldDefinition(string Name, string Label, string Type = "text", bool Required = false);

public sealed class EntityDefinition
{
    public required string Key { get; init; }
    public required string Title { get; init; }
    public required string Table { get; init; }
    public required string SelectSql { get; init; }
    public required string InsertSql { get; init; }
    public required string UpdateSql { get; init; }
    public required string DeleteSql { get; init; }
    public required FieldDefinition[] Fields { get; init; }
    public required string[] InsertParameters { get; init; }
    public required string[] UpdateParameters { get; init; }
}
