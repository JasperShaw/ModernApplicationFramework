using System;
using System.Windows;
using ModernApplicationFramework.Controls.Utilities;
using ModernApplicationFramework.Native.Standard;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Controls.Dialogs
{
    public partial class TextInputDialog
    {
        private TextInputDialog(TextInputViewModel dataContext)
        {
            Validate.IsNotNull(dataContext, "dataContext");
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
            DataContext = dataContext;
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            Dispatcher.BeginInvoke((Action)(() => InputTextBox?.SelectAll()));
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            DataContext = null;
            DataContextChanged -= OnDataContextChanged;
        }

        private void OnRequestClose(object sender, EventArgs e)
        {
            Close();
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            HelperMethods.HandlePropertyChange(e, newVm => newVm.RequestClose += OnRequestClose, (Action<TextInputViewModel>)(oldVm => oldVm.RequestClose -= OnRequestClose));
        }

        public static bool Show(string title, string prompt, string defaultText, out string text)
        {
            return Show(new TextInputViewModel(title, prompt, defaultText), out text);
        }

        public static bool Show(string title, string prompt, string defaultText, Predicate<string> validator, out string text)
        {
            return Show(new TextInputViewModel(title, prompt, defaultText, validator), out text);
        }

        public static bool Show(string title, string prompt, int maxLength, string defaultText, Predicate<string> validator, out string text)
        {
            return Show(new TextInputViewModel(title, prompt, maxLength, defaultText, validator), out text);
        }

        private static bool Show(TextInputViewModel dataContext, out string text)
        {
            new TextInputDialog(dataContext).ShowModal();
            if (dataContext.IsCancelled)
            {
                text = null;
                return false;
            }
            text = dataContext.Text;
            return true;
        }
    }
}
