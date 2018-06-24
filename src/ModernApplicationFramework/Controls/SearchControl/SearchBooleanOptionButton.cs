using System.Windows;
using System.Windows.Input;
using ModernApplicationFramework.Interfaces.Controls.Search;

namespace ModernApplicationFramework.Controls.SearchControl
{
    public class SearchBooleanOptionButton : System.Windows.Controls.CheckBox, ISearchControlPopupLocation
    {
        static SearchBooleanOptionButton()
        {
            EventManager.RegisterClassHandler(typeof(SearchBooleanOptionButton), ClickEvent, new RoutedEventHandler(OnCheckboxClickedHandler));
        }

        void ISearchControlPopupLocation.OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Return || e.Key == Key.Space)
            {
                var isChecked = IsChecked;
                IsChecked = !isChecked;
                OnClick();
                e.Handled = true;
            }
            else if (e.Key == Key.OemPlus || e.Key == Key.Add)
            {
                IsChecked = true;
                OnClick();
                e.Handled = true;
            }
            else
            {
                if (e.Key != Key.OemMinus && e.Key != Key.Subtract)
                    return;
                IsChecked = false;
                OnClick();
                e.Handled = true;
            }
        }

        protected override void OnClick()
        {
            OnToggle();
        }

        protected override void OnToggle()
        {
            RaiseEvent(new RoutedEventArgs(SearchOptionButton.OptionButtonClickedEvent));
        }

        private static void OnCheckboxClickedHandler(object sender, RoutedEventArgs e)
        {
            ((SearchBooleanOptionButton)sender).OnClick();
            e.Handled = true;
        }
    }
}
