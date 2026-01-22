using System.CommandLine;
using System.CommandLine.Parsing;
using System.IO.Hashing;
using System.Text.Json;
using MemoryPack;

class Program
{
    static Types.MediaCatalog LoadMediaCatalog(string path)
    {
        if (Path.GetExtension(path) == ".json")
        {
            return JsonSerializer.Deserialize(File.ReadAllText(path), CatalogJsonContext.Default.MediaCatalog)!;
        }
        else
        {
            return MemoryPackSerializer.Deserialize<Types.MediaCatalog>(File.ReadAllBytes(path))!;
        }
    }
    static Types.TableCatalog LoadTableCatalog(string path)
    {
        if (Path.GetExtension(path) == ".json")
        {
            return JsonSerializer.Deserialize(File.ReadAllText(path), CatalogJsonContext.Default.TableCatalog)!;
        }
        else
        {
            return MemoryPackSerializer.Deserialize<Types.TableCatalog>(File.ReadAllBytes(path))!;
        }
    }
    static void SaveMediaCatalogToJson(Types.MediaCatalog catalog, string path)
    {
        File.WriteAllText(
            path,
            JsonSerializer.Serialize(
                catalog,
                CatalogJsonContext.Default.MediaCatalog));
    }
    static void SaveTableCatalogToJson(Types.TableCatalog catalog, string path)
    {
        File.WriteAllText(
            path,
            JsonSerializer.Serialize(
                catalog,
                CatalogJsonContext.Default.TableCatalog));
    }
    static void SaveMediaCatalogToMemoryPack(Types.MediaCatalog catalog, string path)
    {
        File.WriteAllBytes(
            path,
            MemoryPackSerializer.Serialize(catalog));
    }
    static void SaveTableCatalogToMemoryPack(Types.TableCatalog catalog, string path)
    {
        File.WriteAllBytes(
            path,
            MemoryPackSerializer.Serialize(catalog));
    }
    static int Main(string[] args)
    {
        Option<FileInfo> InputOption = new("--input")
        {
            Description = "The file to read",
            Required = true
        };
        InputOption.Validators.Add(result =>
            {
                if (result.Tokens.Count == 0)
                {
                    result.AddError("Input file is required");
                }
                else
                {
                    var path = result.Tokens[0].Value;
                    var suffix = Path.GetExtension(path);
                    if (File.Exists(path))
                    {
                        if (suffix != ".json" && suffix != ".bytes")
                        {
                            result.AddError("Input file must be .json or .bytes");
                        }
                    }
                    else
                    {
                        result.AddError("Input file does not exist");
                    }
                }
            });
        Option<DirectoryInfo> OutputOption = new("--output")
        {
            Description = "The output directory",
            DefaultValueFactory = result =>
            {
                string path;
                if (result.Tokens.Count == 0)
                {
                    path = Path.Join(Directory.GetCurrentDirectory(), "output");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                }
                else
                {
                    path = result.Tokens[0].Value;
                    var directory = new DirectoryInfo(path);
                    if (!directory.Exists)
                    {
                        directory.Create();
                    }
                }
                return new DirectoryInfo(path);
            }
        };
        Option<int> ThreadOption = new("--thread")
        {
            Description = "The max threads for copy files [1,8]",
            DefaultValueFactory = _ => { return 4; }
        };
        ThreadOption.Validators.Add(result =>
        {
            if (result.Tokens.Count != 0)
            {
                var ret = int.TryParse(result.Tokens[0].Value, out int num);
                if (!ret)
                {
                    result.AddError("Not a number");
                    return;
                }
                if (num < 1 || num > 8)
                {
                    result.AddError("The number is out of range");
                    return;
                }
            }
        });
        RootCommand rootCommand = new("Catalog format converter");
        Command convertCommand = new("convert", "Convert catalog format")
        {
            InputOption,
            OutputOption
        };
        Command exportCommand = new("export", "Export catalog to files")
        {
            InputOption,
            OutputOption,
            ThreadOption
        };
        Argument<DirectoryInfo> AssetsArgument = new("assets")
        {
            Description = "The Assets directory (usually [MediaPatch/TableBundles])",
            DefaultValueFactory = result =>
            {
                if (result.Tokens.Count == 0)
                {
                    result.AddError("Assets directory is required.");
                    return null!;
                }
                else
                {
                    var path = result.Tokens[0].Value;
                    var directory = new DirectoryInfo(path);
                    if (directory.Exists)
                    {
                        return directory;
                    }
                    else
                    {
                        result.AddError("Assets directory does not exist.");
                        return null!;
                    }
                }
            }
        };
        exportCommand.Arguments.Add(AssetsArgument);
        rootCommand.SetAction((action) =>
        {
            Console.WriteLine("Use --help to see available commands.");
            return 0;
        });
        convertCommand.SetAction(action =>
        {
            if (action.Errors.Count != 0)
            {
                foreach (ParseError error in action.Errors)
                {
                    Console.Error.WriteLine(error.Message);
                }
                return 1;
            }
            else
            {
                Console.WriteLine("Start to convert...");
                bool isJson;
                bool isMedia;
                var inputFile = action.GetValue(InputOption)!.FullName;
                var outputDir = action.GetValue(OutputOption)!.FullName;
                string outputFile;
                if (Path.GetExtension(inputFile) == ".json")
                {
                    isJson = true;
                    Console.WriteLine("Turn format from JSON to MemoryPack...");
                }
                else
                {
                    isJson = false;
                    Console.WriteLine("Turn format from MemoryPack to JSON...");
                }
                if (Path.GetFileNameWithoutExtension(inputFile).Contains("table", StringComparison.CurrentCultureIgnoreCase))
                {
                    isMedia = false;
                    Console.WriteLine("Type: TableCatalog");
                }
                else
                {
                    isMedia = true;
                    Console.WriteLine("Type: MediaCatalog");
                }
                if (isMedia)
                {
                    if (isJson)
                    {
                        outputFile = Path.Join(outputDir, "MediaCatalog.bytes");
                        SaveMediaCatalogToMemoryPack(LoadMediaCatalog(inputFile), outputFile);
                    }
                    else
                    {
                        outputFile = Path.Join(outputDir, "MediaCatalog.json");
                        SaveMediaCatalogToJson(LoadMediaCatalog(inputFile), outputFile);
                    }
                }
                else
                {
                    if (isJson)
                    {
                        outputFile = Path.Join(outputDir, "TableCatalog.bytes");
                        SaveTableCatalogToMemoryPack(LoadTableCatalog(inputFile), outputFile);
                    }
                    else
                    {
                        outputFile = Path.Join(outputDir, "TableCatalog.json");
                        SaveTableCatalogToJson(LoadTableCatalog(inputFile), outputFile);
                    }
                }
                Console.WriteLine($"Done!\nFile saved at: {outputFile}");
            }
            return 0;
        });
        exportCommand.SetAction(action =>
        {
            if (action.Errors.Count != 0)
            {
                foreach (ParseError error in action.Errors)
                {
                    Console.Error.WriteLine(error.Message);
                }
                return 1;
            }
            else
            {
                var inputFile = action.GetValue(InputOption)!.FullName;
                var outputDir = action.GetValue(OutputOption)!.FullName;
                var assetsDir = action.GetValue(AssetsArgument)!;
                var threadNum = action.GetValue(ThreadOption);
                Dictionary<string, string> fileMap = [];
                if (Path.GetFileNameWithoutExtension(inputFile).Contains("table", StringComparison.CurrentCultureIgnoreCase))
                {
                    foreach (var item in LoadTableCatalog(inputFile).Table)
                    {
                        var crc = item.Value.Crc.ToString();
                        if (fileMap.TryGetValue(crc, out string? value))
                        {
                            Console.WriteLine($"Warning: Same crc file: ({crc}) '{value}' '{item.Key}'");
                            fileMap[crc] = item.Value.Name;
                        }
                        else
                        {
                            fileMap.Add(crc, item.Value.Name);
                        }
                    }
                }
                else
                {
                    foreach (var item in LoadMediaCatalog(inputFile).Table)
                    {
                        var crc = item.Value.Crc.ToString();
                        if (fileMap.TryGetValue(crc, out string? value))
                        {
                            Console.WriteLine($"Warning: Same crc file: ({crc}) '{value}' '{item.Key}'");
                            fileMap[crc] = item.Value.Path;
                        }
                        else
                        {
                            fileMap.Add(crc, item.Value.Path);
                        }
                    }
                }
                Parallel.ForEach(assetsDir.EnumerateFiles().ToArray(), new ParallelOptions
                {
                    MaxDegreeOfParallelism = threadNum
                }, fileInfo =>
                {
                    var name = fileInfo.Name;
                    var parts = name.Split('_', StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length < 2)
                        return;
                    var crcFromName = parts[1];
                    if (!fileMap.TryGetValue(crcFromName, out var relativePath))
                    {
                        return;
                    }
                    var outputFile = Path.Combine(outputDir, relativePath);
                    var parentDir = Path.GetDirectoryName(outputFile)!;
                    Directory.CreateDirectory(parentDir);
                    if (File.Exists(outputFile))
                    {
                        var outInfo = new FileInfo(outputFile);
                        if (outInfo.Length == fileInfo.Length)
                        {
                            using var stream = File.OpenRead(outputFile);
                            var crc32 = new Crc32(); crc32.Append(stream); var crcOut = crc32.GetCurrentHashAsUInt32().ToString();
                            if (crcOut == crcFromName)
                            {
                                lock (Console.Out)
                                {
                                    Console.WriteLine($"Info: Skip copy '{outputFile}'");
                                }
                                return;
                            }
                        }
                    }
                    lock (Console.Out)
                    {
                        Console.WriteLine($"{name} => {outputFile}");
                    }
                    try
                    {
                        File.Copy(fileInfo.FullName, outputFile, overwrite: true);
                    }
                    catch (Exception error)
                    {
                        lock (Console.Out)
                        {
                            Console.WriteLine($"Error: Copy failed '{outputFile}' - {error.Message}");
                        }
                    }
                });
                Console.WriteLine($"Done!\nFiles saved at: {outputDir}");
            }
            return 0;
        });
        rootCommand.Subcommands.Add(convertCommand);
        rootCommand.Subcommands.Add(exportCommand);
        return rootCommand.Parse(args).Invoke();
    }
}
