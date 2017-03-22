using ModernApplicationFramework.Native.TrinetCoreNtfs;

namespace ModernApplicationFramework.Native.Platform.Structs
{
    internal struct Win32StreamInfo
    {
        public FileStreamType StreamType;
        public FileStreamAttributes StreamAttributes;
        public long StreamSize;
        public string StreamName;
    }
}
