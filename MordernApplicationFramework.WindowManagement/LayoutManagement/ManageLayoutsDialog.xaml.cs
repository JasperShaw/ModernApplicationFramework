using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Caliburn.Micro;
using ModernApplicationFramework.Utilities;
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

        internal static IEnumerable<KeyValuePair<string, WindowLayoutInfo>> Show(
            IEnumerable<KeyValuePair<string, WindowLayoutInfo>> layoutKeyInfoCollection)
        {
            var dataContext = new ManageLayoutsViewModel(layoutKeyInfoCollection);
            Show(dataContext);
            return dataContext.Layouts.Select(
                layout => new KeyValuePair<string, WindowLayoutInfo>(layout.Key, layout.Info));
        }

        private static void Show(ManageLayoutsViewModel dataContext)
        {
            Validate.IsNotNull(dataContext, "DataContext");
            var manageLayoutsDialog = new ManageLayoutsDialog();
            var settings = IoC.Get<IWindowLayoutSettings>();
            if (settings == null)
                return;
            var layoutsDialogWidth = settings.ManageLayoutsDialogWidth;
            if (layoutsDialogWidth != 0)
                manageLayoutsDialog.Width = layoutsDialogWidth;
            var layoutsDialogHeight = settings.ManageLayoutsDialogHeight;
            if (layoutsDialogHeight != 0)
                manageLayoutsDialog.Height = layoutsDialogHeight;
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
            var frameworkElement = LayoutList.ItemContainerGenerator.ContainerFromItem(LayoutList.SelectedItem) as FrameworkElement;
            if (frameworkElement == null)
                return;
            Keyboard.Focus(frameworkElement);
        }
    }
}
