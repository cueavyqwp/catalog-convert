using System.Text.Json;

using MemoryPack;

class Program
{

    static int Main(string[] args)
    {
        string output_file = "";
        bool is_json = false;
        if (args.Length < 1)
        {
            Console.WriteLine("At least input a catalog file.");
            return 1;
        }
        if (args.Length >= 2)
        {
            output_file = args[2];
        }
        string source_file = args[0];
        var type = Path.GetFileNameWithoutExtension(source_file) == "TableCatalog" ? typeof(Types.TableCatalog) : typeof(Types.MediaCatalog);
        Console.WriteLine($"From:\t'{source_file}'");
        if (!File.Exists(source_file))
        {
            Console.WriteLine("The catalog file not found.");
            return 2;
        }
        if (Path.GetExtension(source_file) == ".json")
        {
            is_json = true;
        }
        if (output_file == "")
        {
            output_file = Path.Combine(Path.GetDirectoryName(source_file) ?? string.Empty, Path.GetFileNameWithoutExtension(source_file) + (is_json ? ".bytes" : ".json"));
        }
        Console.WriteLine($"To:\t'{output_file}'");
        if (is_json)
        {
            File.WriteAllBytes(output_file, MemoryPackSerializer.Serialize(type, JsonSerializer.Deserialize(File.ReadAllText(source_file), type)));
        }
        else
        {
            File.WriteAllText(output_file, JsonSerializer.Serialize(MemoryPackSerializer.Deserialize(type, File.ReadAllBytes(source_file)), new JsonSerializerOptions { WriteIndented = true }));
        }
        Console.WriteLine($"Done");
        return 0;
    }
}
