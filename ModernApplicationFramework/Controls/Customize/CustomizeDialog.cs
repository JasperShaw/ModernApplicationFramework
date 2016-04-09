using System;
using System.Windows;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.ViewModels;

namespace ModernApplicationFramework.Controls.Customize
{
    public class CustomizeDialog : DialogWindow
    {
        private readonly ToolBarHostViewModel _toolBarHostViewModel;
        private bool _contentLoaded;

        public CustomizeDialog(ToolBarHostViewModel toolBarViewModel)
        {
            InitializeComponent();
            _toolBarHostViewModel = toolBarViewModel;
            Loaded += CustomizeDialog_Loaded;
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

        private void CustomizeDialog_Loaded(object sender, RoutedEventArgs e)
        {
            var button = VisualUtilities.FindChild<System.Windows.Controls.Button>(this, "Button");
            if (button == null)
                return;
            button.Click += OnCloseButtonClick;

            var toolBarsPage = VisualUtilities.FindChild<ToolBarsPage>(this, "ToolBarsPage");

            var m = toolBarsPage?.DataContext as ToolBarCustomizeDialogViewModel;
            if (m != null)
                m.ToolBarHostViewModel = _toolBarHostViewModel;
        }

        private void OnCloseButtonClick(object sender, RoutedEventArgs routedEventArgs)
        {
            Close();
        }
    }
}