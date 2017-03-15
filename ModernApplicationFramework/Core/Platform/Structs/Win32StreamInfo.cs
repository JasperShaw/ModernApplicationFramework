using ModernApplicationFramework.Core.TrinetCoreNtfs;

namespace ModernApplicationFramework.Core.Platform.Structs
{
    internal struct Win32StreamInfo
    {
        public FileStreamType StreamType;
        public FileStreamAttributes StreamAttributes;
        public long StreamSize;
        public string StreamName;
    }
}
