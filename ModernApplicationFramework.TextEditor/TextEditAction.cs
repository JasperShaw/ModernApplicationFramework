namespace ModernApplicationFramework.TextEditor
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