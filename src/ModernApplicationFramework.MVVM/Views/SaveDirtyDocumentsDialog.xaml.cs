using System;
using System.Collections.Generic;
using System.Windows;
using ModernApplicationFramework.MVVM.Core;
using ModernApplicationFramework.MVVM.Core.NativeMethods;
using ModernApplicationFramework.MVVM.Interfaces;

namespace ModernApplicationFramework.MVVM.Views
{
    public partial class SaveDirtyDocumentsDialog : ISaveDirtyDocumentsDialog
    {
        public SaveDirtyDocumentsDialog()
        {
            InitializeComponent();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            NativeMethods.RemoveIcon(this);
        }

        public SaveDirtyDocumentsDialog(IEnumerable<SaveDirtyDocumentItem> items)
        {
            ItemSource = items;
            InitializeComponent();
            Title = Application.Current.MainWindow.Title;
        }

        public IEnumerable<SaveDirtyDocumentItem> ItemSource { get; }

        public MessageBoxResult Result { get; private set; }

        public static MessageBoxResult Show(IEnumerable<SaveDirtyDocumentItem> items)
        {
            var windows = new SaveDirtyDocumentsDialog(items);
            windows.ShowDialog();
            return windows.Result;
        }

        private void Button_ClickedYes(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Yes;
            Close();
        }

        private void Button_ClickedNo(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.No;
            Close();
        }

        private void Button_ClickedCancel(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Cancel;
            Close();
        }
    }
}
