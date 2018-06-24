using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ModernApplicationFramework.Interfaces.Controls.Search;

namespace ModernApplicationFramework.Controls.SearchControl
{
    public class SearchCommandOptionButton : Button, ISearchControlPopupLocation
    {
         void ISearchControlPopupLocation.OnKeyDown(KeyEventArgs e)
        {
            if (e.Key != Key.Return && e.Key != Key.Space)
                return;
            OnClick();
            e.Handled = true;
        }

        protected override void OnClick()
        {
            base.OnClick();
            RaiseEvent(new RoutedEventArgs(SearchOptionButton.OptionButtonClickedEvent));
        }
    }
}
