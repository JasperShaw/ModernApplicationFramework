namespace ModernApplicationFramework.Native.Platform.Enums
{
    internal enum Sigdn : uint
    {
        SigdnNormaldisplay = 0x00000000,           // SHGDN_NORMAL
        SigdnParentrelativeparsing = 0x80018001,   // SHGDN_INFOLDER | SHGDN_FORPARSING
        SigdnDesktopabsoluteparsing = 0x80028000,  // SHGDN_FORPARSING
        SigdnParentrelativeediting = 0x80031001,   // SHGDN_INFOLDER | SHGDN_FOREDITING
        SigdnDesktopabsoluteediting = 0x8004c000,  // SHGDN_FORPARSING | SHGDN_FORADDRESSBAR
        SigdnFilesyspath = 0x80058000,             // SHGDN_FORPARSING
        SigdnUrl = 0x80068000,                     // SHGDN_FORPARSING
        SigdnParentrelativeforaddressbar = 0x8007c001,     // SHGDN_INFOLDER | SHGDN_FORPARSING | SHGDN_FORADDRESSBAR
        SigdnParentrelative = 0x80080001           // SHGDN_INFOLDER
    }
}
