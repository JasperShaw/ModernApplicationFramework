namespace ModernApplicationFramework.Modules.Editor.Operations
{
    internal enum TextEditAction
    {
        None,
        Type,
        Delete,
        Backspace,
        Paste,
        Enter,
        AutoIndent,
        Replace,
        ProvisionalOverwrite,
    }
}