namespace ModernApplicationFramework.Controls.InfoBar
{
    public interface IInfoBarTextSpan
    {
        string Text { get; }

        bool Bold { get; }

        bool Italic { get; }

        bool Underline { get; }
    }
}