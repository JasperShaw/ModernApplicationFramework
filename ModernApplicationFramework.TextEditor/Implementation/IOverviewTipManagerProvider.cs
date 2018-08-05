namespace ModernApplicationFramework.TextEditor.Implementation
{
    public interface IOverviewTipManagerProvider
    {
        IOverviewTipManager GetOverviewTipManager(ITextViewHost host);
    }
}