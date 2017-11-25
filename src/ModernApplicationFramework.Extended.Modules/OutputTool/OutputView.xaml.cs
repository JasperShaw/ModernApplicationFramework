using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Extended.Modules.OutputTool
{
    public partial class OutputView : IOutputView
    {
        public OutputView()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            MoveFocusToActivePane();
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(e.NewValue is INotifyPropertyChanged newValue))
                return;
            newValue.PropertyChanged += OnDataSourcePropertyChanged;
        }

        private void OnDataSourcePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "IsSelected" || !IsKeyboardFocusWithin)
                return;
            MoveFocusToActivePane();
        }

        private void MoveFocusToActivePane()
        {
            FrameworkElement paneFromDataSource = GetActivePaneFromDataSource();
            if (paneFromDataSource == null)
                return;
            PendingFocusHelper.SetFocusOnLoad(paneFromDataSource);
        }

        private FrameworkElement GetActivePaneFromDataSource()
        {
            if (!(DataContext is ILayoutItemBase dataContext))
                return null;
            return dataContext.IsSelected ? this : null;
        }

        public void ScrollToEnd()
        {
            OutputText.CaretIndex = OutputText.Text.Length;
            ScrollViewer.ScrollToEnd();
        }

        public void Clear()
        {
            OutputText.Clear();
        }

        public void AppendText(string text)
        {
            OutputText.AppendText(text);
            ScrollToEnd();
        }

        public void SetText(string text)
        {
            OutputText.Text = text;
            ScrollToEnd();
        }
    }
}