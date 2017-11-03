namespace ModernApplicationFramework.Controls.InfoBar
{
    public interface IInfoBarTextSpanCollection
    {
        int Count { get; }

        IInfoBarTextSpan GetSpan(int index);
    }
}