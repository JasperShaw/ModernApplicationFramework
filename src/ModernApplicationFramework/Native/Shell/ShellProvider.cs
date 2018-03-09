// ReSharper disable InconsistentNaming
namespace ModernApplicationFramework.Native.Shell
{
    internal enum SIAttribFlags
    {
        And = 0x00000001,
        Or = 0x00000002,
        AppCompat = 0x00000003
    }

    public enum SIGDN : uint
    {
        // lower word (& with 0xFFFF)
        NormalDisplay = 0x00000000, // SHGDN_NORMAL
        ParentRelativeParsing = 0x80018001, // SHGDN_INFOLDER | SHGDN_FORPARSING
        DesktopAbsoluteParsing = 0x80028000, // SHGDN_FORPARSING
        ParentRelativeEditing = 0x80031001, // SHGDN_INFOLDER | SHGDN_FOREDITING
        DesktopAbsoluteEditing = 0x8004c000, // SHGDN_FORPARSING | SHGDN_FORADDRESSBAR
        FileSysPath = 0x80058000, // SHGDN_FORPARSING
        Url = 0x80068000, // SHGDN_FORPARSING
        ParentRelativeForAddressbar = 0x8007c001, // SHGDN_INFOLDER | SHGDN_FORPARSING | SHGDN_FORADDRESSBAR
        ParentRelative = 0x80080001 // SHGDN_INFOLDER
    }
}