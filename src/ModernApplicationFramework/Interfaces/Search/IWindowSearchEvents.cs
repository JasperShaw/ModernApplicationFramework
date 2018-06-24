using ModernApplicationFramework.Basics.Search;

namespace ModernApplicationFramework.Interfaces.Search
{
    public interface IWindowSearchEvents
    {
        void SearchOptionsChanged();

        void SearchOptionValueChanged(IWindowSearchBooleanOption option);
    }
}