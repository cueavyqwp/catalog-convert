using MemoryPack;

namespace Types
{
    public enum MediaType : int
    {
        None = 0,
        Audio = 1,
        Video = 2,
        Texture = 3
    }
    [MemoryPackable]
    public partial class Media
    {
        public required string Path { get; set; }
        public required string FileName { get; set; }
        public long Bytes { get; set; }
        public long Crc { get; set; }
        public bool IsPrologue { get; set; }
        public bool IsSplitDownload { get; set; }
        public MediaType MediaType { get; set; }
    }

    [MemoryPackable]
    public partial class MediaCatalog
    {
        public required Dictionary<string, Media> Table { get; set; }
    }
    [MemoryPackable]
    public partial class TableBundle
    {
        public required string Name { get; set; }
        public long Size { get; set; }
        public long Crc { get; set; }
        public bool isInbuild { get; set; }
        public bool isChanged { get; set; }
        public bool IsPrologue { get; set; }
        public bool IsSplitDownload { get; set; }
        public required List<string> Includes { get; set; }
    }

    [MemoryPackable]
    public partial class TablePatchPack
    {
        public required string Name { get; set; }
        public long Size { get; set; }
        public long Crc { get; set; }
        public bool IsPrologue { get; set; }
        public required TableBundle[] BundleFiles { get; set; }
    }

    [MemoryPackable]
    public partial class TableCatalog
    {
        public required Dictionary<string, TableBundle> Table { get; set; }
        public required Dictionary<string, TablePatchPack> TablePack { get; set; }
    }
}
