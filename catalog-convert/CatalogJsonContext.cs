using System.Text.Json.Serialization;

[JsonSerializable(typeof(Types.TableCatalog))]
[JsonSerializable(typeof(Types.MediaCatalog))]
public partial class CatalogJsonContext : JsonSerializerContext
{
}
