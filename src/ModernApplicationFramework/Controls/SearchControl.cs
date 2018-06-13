using System.Windows;
using System.Windows.Controls;

namespace ModernApplicationFramework.Controls
{
    public class SearchControl : Control
    {
        static SearchControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SearchControl), new FrameworkPropertyMetadata(typeof(SearchControl)));
        }
    }
}
