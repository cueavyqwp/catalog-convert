<div align = "center" >
    <h1>catalog-convert</h1>

Inspired by [Blue-Archive-Catalog-Converter](https://github.com/endergreen12/Blue-Archive-Catalog-Converter)

</div>

---

# Download

Go to the [Release](https://github.com/cueavyqwp/catalog-convert/releases)

`catalog-convert.exe` is for Windows x64

`catalog-convert-x64` is for Linux x64

`catalog-convert-arm64` is for Linux arm64

You also can build by yourself ([See at here](#build))

# Use

## Help

```bash
./catalog-convert -h
./catalog-convert -h convert
./catalog-convert -h export
```

## Convert

Convert MediaCatalog.bytes to MediaCatalog.json, TableCatalog.bytes to TableCatalog.json or reverse.

```bash
./catalog-convert convert -i [source_file]
```

## Export

For MediaPatch

```bash
./catalog-convert export -i [MediaCatalog.bytes/MediaCatalog.json] [BlueArchive_JP/BlueArchive_Data/StreamingAssets/MediaPatch]
```

For TableBundles

```bash
./catalog-convert export -i [TableCatalog.bytes/TableCatalog.json] [BlueArchive_JP/BlueArchive_Data/StreamingAssets/TableBundles]
```

# Build

Make sure that you already installed [.Net SDK 10](https://dotnet.microsoft.com/download/dotnet/10.0)

`dotnet publish ./catalog-convert --configuration Release --framework net10.0 -o ./build -r win-x64 --self-contained true /p:PublishTrimmed=true`

You can change the `win-x64` to other platform

Windows
- win-x64
- win
- any

Linux
- linux-x64
- linux
- unix-x64
- unix
- any

macOS
- osx-x64
- osx
- unix-x64
- unix

# Thanks to

- [endergreen12/Blue-Archive-Catalog-Converter](https://github.com/endergreen12/Blue-Archive-Catalog-Converter)
- [KaniArchive/MemoryPackDumper](https://github.com/KaniArchive/MemoryPackDumper)
- [LukeFZ/Il2CppInspectorRedux](https://github.com/LukeFZ/Il2CppInspectorRedux)
- [Deathemonic/BA-AX](https://github.com/Deathemonic/BA-AX)
