namespace ModernApplicationFramework.Interfaces.Controls.InfoBar
{
    public interface IInfoBarActionItemCollection
    {
        int Count { get; }

        IInfoBarActionItem GetItem(int index);       
    }
}