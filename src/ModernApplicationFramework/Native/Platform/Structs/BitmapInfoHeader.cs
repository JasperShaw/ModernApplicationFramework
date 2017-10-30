namespace ModernApplicationFramework.Native.Platform.Structs
{
    internal struct Bitmapinfoheader
    {
        internal uint BiSize;
        internal int BiWidth;
        internal int BiHeight;
        internal ushort BiPlanes;
        internal ushort BiBitCount;
        internal uint BiCompression;
        internal uint BiSizeImage;
        internal int BiXPelsPerMeter;
        internal int BiYPelsPerMeter;
        internal uint BiClrUsed;
        internal uint BiClrImportant;

        internal static Bitmapinfoheader Default => new Bitmapinfoheader
        {
            BiSize = 40,
            BiPlanes = 1
        };
    }
}
