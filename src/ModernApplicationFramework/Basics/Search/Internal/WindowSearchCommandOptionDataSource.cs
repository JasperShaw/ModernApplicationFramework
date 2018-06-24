using ModernApplicationFramework.Interfaces.Search;

namespace ModernApplicationFramework.Basics.Search.Internal
{
    internal class WindowSearchCommandOptionDataSource : WindowSearchOptionDataSource
    {
        public WindowSearchCommandOptionDataSource(IWindowSearchCommandOption option)
            : base(option)
        {
            Type = SearchOptionType.Command;
        }

        protected override void OnSelect()
        {
            ((IWindowSearchCommandOption)Option).Invoke();
        }
    }
}