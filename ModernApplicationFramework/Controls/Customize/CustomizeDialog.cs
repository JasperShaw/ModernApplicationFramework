using System;
using System.Windows;
using ModernApplicationFramework.Core.Utilities;

namespace ModernApplicationFramework.Controls.Customize
{
    public class CustomizeDialog : DialogWindow
    {
        private bool _contentLoaded;

        public CustomizeDialog()
        {
            InitializeComponent();
            Loaded += CustomizeDialog_Loaded;
        }

        private void CustomizeDialog_Loaded(object sender, RoutedEventArgs e)
        {
            var button = VisualUtilities.FindChild<System.Windows.Controls.Button>(this, "Button");
            if (button == null)
                return;
            button.Click += OnCloseButtonClick;
        }

        private void OnCloseButtonClick(object sender, RoutedEventArgs routedEventArgs)
        {
            Close();
        }

        public void InitializeComponent()
        {
            if (_contentLoaded)
                return;
            _contentLoaded = true;
            Application.LoadComponent(this,
                new Uri("/ModernApplicationFramework;component/Controls/Customize/customizedialog.xaml",
                    UriKind.Relative));
        }
    }
}
