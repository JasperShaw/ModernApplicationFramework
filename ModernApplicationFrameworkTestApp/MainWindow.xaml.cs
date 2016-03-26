using System.Windows;
using System.Windows.Input;
using ModernApplicationFramework.Controls;
using ModernApplicationFramework.Core.Events;
using ModernApplicationFramework.Themes;
using ModernApplicationFramework.Themes.LightIDE;

namespace ModernApplicationFrameworkTestApp
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new LightTheme().GetResourceUri()
            });
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Application.Current.Resources.MergedDictionaries.Clear();

            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new GenericTheme().GetResourceUri()
            });
        }

        private void DropDownButton_OnClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Test");
        }

        private void TextBox_OnPrevieTextChanged(object sender, PreviewTextChangedEventArgs e)
        {
            MessageBox.Show(TextBox.Text);
        }

        private void MainWindow_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var a = new ContextMenu { HasDropShadow = true };
            a.Items.Add(new ContextMenuItem { Header = "Test", InputGestureText = "Testing"});
            a.Items.Add(new ContextMenuItem { Header = "Test" });
            a.Items.Add(new ContextMenuItem { Header = "Test" });
            a.Items.Add(new ContextMenuItem { Header = "Test" });
            a.Items.Add(new Separator());
            a.Items.Add(new ContextMenuItem { Header = "Test" });
            a.Items.Add(new ContextMenuItem { Header = "Test" });
            a.Items.Add(new ContextMenuItem { Header = "Test" });
            a.IsOpen = true;
        }
    }
}
