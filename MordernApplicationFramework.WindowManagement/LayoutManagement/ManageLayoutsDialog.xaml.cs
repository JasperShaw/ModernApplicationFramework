using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using ModernApplicationFramework.Controls.Dialogs;
using ModernApplicationFramework.Utilities;
using MordernApplicationFramework.WindowManagement.Properties;
using Action = System.Action;

namespace MordernApplicationFramework.WindowManagement.LayoutManagement
{
    public partial class ManageLayoutsDialog
    {
        private bool _scrollPending;

        private ManageLayoutsDialog()
        {
            InitializeComponent();
            FocusHelper.FocusPossiblyUnloadedElement(LayoutList);
        }

        internal static IEnumerable<KeyValuePair<string, WindowLayout>> Show(
            IEnumerable<KeyValuePair<string, WindowLayout>> layoutKeyInfoCollection)
        {
            var dataContext = new ManageLayoutsViewModel(layoutKeyInfoCollection, new DialogUserInput());
            Show(dataContext);
            return dataContext.Layouts.Select(
                layout => new KeyValuePair<string, WindowLayout>(layout.Key, layout.Layout));
        }

        internal static IEnumerable<KeyValuePair<string, WindowLayout>> Show(IEnumerable<KeyValuePair<string, WindowLayout>> layoutKeyInfoCollection, IWindowLayoutSettings settings = null)
        {
            ManageLayoutsViewModel dataContext = new ManageLayoutsViewModel(layoutKeyInfoCollection, new DialogUserInput());
            Show(dataContext, settings);
            return dataContext.Layouts.Select(layout => new KeyValuePair<string, WindowLayout>(layout.Key, layout.Layout));
        }

        private static void Show(ManageLayoutsViewModel dataContext, IWindowLayoutSettings settings = null)
        {
            Validate.IsNotNull(dataContext, "DataContext");
            var manageLayoutsDialog = new ManageLayoutsDialog();
            if (settings != null)
            {
                var layoutsDialogWidth = settings.ManageLayoutsDialogWidth;
                if (layoutsDialogWidth != 0)
                    manageLayoutsDialog.Width = layoutsDialogWidth;
                var layoutsDialogHeight = settings.ManageLayoutsDialogHeight;
                if (layoutsDialogHeight != 0)
                    manageLayoutsDialog.Height = layoutsDialogHeight;
            }       
            manageLayoutsDialog.DataContext = dataContext;
            dataContext.Layouts.CollectionChanged += manageLayoutsDialog.OnLayoutsCollectionChanged;
            try
            {
                manageLayoutsDialog.ShowModal();
            }
            finally
            {
                dataContext.Layouts.CollectionChanged -= manageLayoutsDialog.OnLayoutsCollectionChanged;
            }
            if (settings == null)
                return;
            settings.ManageLayoutsDialogWidth = (int) manageLayoutsDialog.Width;
            settings.ManageLayoutsDialogHeight = (int) manageLayoutsDialog.Height;
        }

        private void OnLayoutsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_scrollPending)
                return;
            _scrollPending = true;
            Dispatcher.BeginInvoke((Action)(() =>
            {
                TryScrollToSelectedItem();
                _scrollPending = false;
            }), DispatcherPriority.Loaded);
        }

        private void OnLayoutListGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!Equals(Keyboard.FocusedElement, LayoutList))
                return;
            Dispatcher.BeginInvoke((Action)(() =>
            {
                if (!Equals(Keyboard.FocusedElement, LayoutList))
                    return;
                FocusSelectedItem();
            }), DispatcherPriority.Loaded);
        }

        private bool TryScrollToSelectedItem()
        {
            var selectedItem = LayoutList.SelectedItem;
            if (selectedItem == null)
                return false;
            LayoutList.ScrollIntoView(selectedItem);
            return true;
        }

        private void FocusSelectedItem()
        {
            if (!TryScrollToSelectedItem())
                return;
            if (!(LayoutList.ItemContainerGenerator.ContainerFromItem(LayoutList.SelectedItem) is FrameworkElement frameworkElement))
                return;
            Keyboard.Focus(frameworkElement);
        }

        private class DialogUserInput : ILayoutManagementUserManageInput
        {
            public bool GetRenamedLayoutName(string defaultName, Predicate<string> nameValidator, out string layoutName)
            {
                Validate.IsNotNull(defaultName, nameof(defaultName));
                Validate.IsNotNull(nameValidator, nameof(nameValidator));
                return TextInputDialog.Show(WindowManagement_Resources.RenameLayoutTitle, WindowManagement_Resources.RenameLayoutMessage, 100, defaultName, nameValidator, out layoutName);
            }

            public bool GetReplaceLayoutConfirmation(string name)
            {
                return MessageDialog.Show(WindowManagement_Resources.RenameLayoutTitle, string.Format(CultureInfo.CurrentUICulture, WindowManagement_Resources.LayoutOverwriteMessage, new[]
                {
                    (object) name
                }), MessageDialogCommandSet.YesNo) == MessageDialogCommand.Yes;
            }

            public bool GetDeleteLayoutConfirmation(string name)
            {
                return MessageDialog.Show(WindowManagement_Resources.DeleteLayoutTitle, string.Format(CultureInfo.CurrentUICulture, WindowManagement_Resources.DeleteLayoutConfirmation, new[]
                {
                    (object) name
                }), MessageDialogCommandSet.YesNo) == MessageDialogCommand.Yes;
            }
        }
    }
}
