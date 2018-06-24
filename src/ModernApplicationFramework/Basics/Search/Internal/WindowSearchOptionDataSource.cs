using System;
using ModernApplicationFramework.Interfaces.Search;

namespace ModernApplicationFramework.Basics.Search.Internal
{
    internal class WindowSearchOptionDataSource : SearchOptionDataSource
    {
        internal IWindowSearchOption Option { get; }

        public WindowSearchOptionDataSource(IWindowSearchOption option)
        {
            Option = option ?? throw new ArgumentNullException(nameof(option));
            DisplayText = option.DisplayText;
            Tooltip = option.Tooltip;
        }
    }
}