using System.Windows;

namespace ModernApplicationFramework.Test
{
    /// <summary>
    /// Interaktionslogik für TabView.xaml
    /// </summary>
    public partial class TabView
    {
        public TabView()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            ToolBarListBox.Focus();
            Loaded -= OnLoaded;
        }
    }
}
