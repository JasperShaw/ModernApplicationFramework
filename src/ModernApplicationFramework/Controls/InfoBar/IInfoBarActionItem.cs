namespace ModernApplicationFramework.Controls.InfoBar
{
    public interface IInfoBarActionItem : IInfoBarTextSpan
    {
        new string Text { get; }

        new bool Bold { get; }

        new bool Italic { get; }

        new bool Underline { get; }

        object ActionContext { get; }

        bool IsButton { get; }
    }
}