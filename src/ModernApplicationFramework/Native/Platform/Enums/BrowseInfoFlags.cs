using System;

namespace ModernApplicationFramework.Native.Platform.Enums
{
    [Flags]
    public enum BrowseInfoFlags
    {
        ReturnOnlyFsDirs = 0x00000001,
        DontGoBelowDomain = 0x00000002,
        StatusText = 0x00000004,
        ReturnFsAncestors = 0x00000008,
        EditBox = 0x00000010,
        Validate = 0x00000020,
        NewDialogStyle = 0x00000040,
        UseNewUi = NewDialogStyle | EditBox,
        BrowseIncludeUrls = 0x00000080,
        UaHint = 0x00000100,
        NoNewFolderButton = 0x00000200,
        NoTranslateTargets = 0x00000400,
        BrowseForComputer = 0x00001000,
        BrowseForPrinter = 0x00002000,
        BrowseIncludeFiles = 0x00004000,
        Shareable = 0x00008000,
        BrowseFileJunctions = 0x00010000
    }
}
