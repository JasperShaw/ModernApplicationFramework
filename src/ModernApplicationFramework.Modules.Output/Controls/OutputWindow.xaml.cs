using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Modules.Output.Controls
{
    internal partial class OutputWindow
    {
        public OutputWindow()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            MoveFocusToActivePane();
        }

        private void MoveFocusToActivePane()
        {
            var paneFromDataSoure = GeGetActivePaneFromDataSource();
            if (paneFromDataSoure == null)
                return;
            PendingFocusHelper.SetFocusOnLoad(paneFromDataSoure);
        }

        private FrameworkElement GeGetActivePaneFromDataSource()
        {
            if (!(DataContext is IOutputWindowDataSource dataContext))
                return null;
            return dataContext.ActivePane;

        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(e.NewValue is INotifyPropertyChanged newValue))
                return;
            newValue.PropertyChanged += OnDataSourcePropertyChanged;
        }

        private void OnDataSourcePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(IOutputWindowDataSource.ActivePane) || !IsKeyboardFocusWithin)
                return;
            MoveFocusToActivePane();
        }
    }
}
