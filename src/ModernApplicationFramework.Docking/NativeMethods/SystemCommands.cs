namespace ModernApplicationFramework.Docking.NativeMethods
{
    internal enum SystemCommands
    {
        ScSize = 0xF000,
        ScMove = 0xF010,
        ScMinimize = 0xF020,
        ScMaximize = 0xF030,
        ScNextwindow = 0xF040,
        ScPrevwindow = 0xF050,
        ScClose = 0xF060,
        ScVscroll = 0xF070,
        ScHscroll = 0xF080,
        ScMousemenu = 0xF090,
        ScKeymenu = 0xF100,
        ScArrange = 0xF110,
        ScRestore = 0xF120,
        ScTasklist = 0xF130,
        ScScreensave = 0xF140,
        ScHotkey = 0xF150,
        ScDefault = 0xF160,
        ScMonitorpower = 0xF170,
        ScContexthelp = 0xF180,
        ScSeparator = 0xF00F,
        ScfIssecure = 0x00000001,
        ScIcon = ScMinimize,
        ScZoom = ScMaximize,
    }
}
