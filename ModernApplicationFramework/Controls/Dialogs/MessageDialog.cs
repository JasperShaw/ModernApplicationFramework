using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using ModernApplicationFramework.Controls.Utilities;
using ModernApplicationFramework.Native.Standard;
using ModernApplicationFramework.Utilities;
using Button = ModernApplicationFramework.Controls.Buttons.Button;

namespace ModernApplicationFramework.Controls.Dialogs
{
    public class MessageDialog : DialogWindow
    {
        private bool _focusPending;
        internal ItemsControl ButtonList;
        private bool _contentLoaded;

        private MessageDialog(MessageViewModel dataContext)
        {
            Validate.IsNotNull(dataContext, "dataContext");
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
            DataContext = dataContext;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            DataContext = null;
            DataContextChanged -= OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            HelperMethods.HandlePropertyChange(e, newVm => newVm.RequestClose += OnRequestClose, (Action<MessageViewModel>)(oldVm => oldVm.RequestClose -= OnRequestClose));
            if (_focusPending)
                return;
            _focusPending = true;
            Dispatcher.BeginInvoke((Action)(() =>
            {
                var defaultButtonToFocus = GetDefaultButtonToFocus();
                defaultButtonToFocus?.Focus();
                _focusPending = false;
            }));
        }

        private Button GetDefaultButtonToFocus()
        {
            Button button1 = null;
            Button button2 = null;
            foreach (Button descendant in ButtonList.FindDescendants<Button>())
            {
                if (descendant.IsDefault)
                    return descendant;
                if (descendant.IsCancel && button1 == null)
                    button1 = descendant;
                button2 = button2 ?? descendant;
            }
            return button1 ?? button2;
        }

        private void OnRequestClose(object sender, EventArgs e)
        {
            Close();
        }

        public static MessageDialogCommand Show(string title, string message, MessageDialogCommandSet commandSet)
        {
            return Show(new MessageViewModel(title, message, commandSet));
        }

        public static MessageDialogCommand Show(string title, string message, MessageDialogCommandSet commandSet, string confirmationMessage, out bool confirmationResponse)
        {
            var dataContext = new MessageViewModel(title, message, commandSet, confirmationMessage);
            var messageDialogCommand = Show(dataContext);
            confirmationResponse = dataContext.ConfirmationState;
            return messageDialogCommand;
        }

        private static MessageDialogCommand Show(MessageViewModel dataContext)
        {
            new MessageDialog(dataContext).ShowModal();
            return dataContext.Response;
        }

        [DebuggerNonUserCode]
        public void InitializeComponent()
        {
            if (_contentLoaded)
                return;
            _contentLoaded = true;
            Application.LoadComponent(this,
                new Uri("/ModernApplicationFramework;component/Themes/Generic/Dialogs/MessageDialog.xaml", UriKind.Relative));
        }
    }
}
