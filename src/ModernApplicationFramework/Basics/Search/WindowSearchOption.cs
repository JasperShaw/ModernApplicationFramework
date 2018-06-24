using ModernApplicationFramework.Interfaces.Search;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Basics.Search
{
    public class WindowSearchOption : IWindowSearchOption
    {
        public string DisplayText { get; }
        public string Tooltip { get; }

        public WindowSearchOption(string displayText, string tooltip)
        {
            Validate.IsNotNullAndNotWhiteSpace(displayText, nameof(displayText));
            Validate.IsNotWhiteSpace(tooltip, nameof(tooltip));
            DisplayText = displayText;
            Tooltip = tooltip;
        }
    }
}