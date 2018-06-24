using ModernApplicationFramework.Interfaces.Search;

namespace ModernApplicationFramework.Basics.Search.Internal
{
    internal class WindowSearchBooleanOptionDataSource : WindowSearchOptionDataSource
    {
        public WindowSearchBooleanOptionDataSource(IWindowSearchOption option) : base(option)
        {
            Type = SearchOptionType.Boolean;
            Value = ((IWindowSearchBooleanOption) Option).Value;
        }

        protected override void OnSelect()
        {
            ((IWindowSearchBooleanOption) Option).Value = Value;
        }
    }
}