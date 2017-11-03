namespace ModernApplicationFramework.Controls.InfoBar
{
    public interface IInfoBarActionItemCollection
    {
        int Count { get; }

        IInfoBarActionItem GetItem(int index);       
    }
}