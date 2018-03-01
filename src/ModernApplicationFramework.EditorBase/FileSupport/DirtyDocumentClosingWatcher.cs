using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ModernApplicationFramework.EditorBase.Controls.SaveDirtyDocumentsDialog;
using ModernApplicationFramework.EditorBase.Interfaces.Editor;
using ModernApplicationFramework.Extended.Package;

namespace ModernApplicationFramework.EditorBase.FileSupport
{

    [Export(typeof(IMafPackage))]
    internal class DirtyDocumentClosingWatcherPackage : Package
    {
        public override PackageLoadOption LoadOption => PackageLoadOption.OnMainWindowLoaded;
        public override PackageCloseOption CloseOption => PackageCloseOption.OnMainWindowClosed;
        public override Guid Id => new Guid("{A6B83B74-3ED5-49FE-B11C-F7335A9057BC}");

        public override void Initialize()
        {
            base.Initialize();
            SetupEventHandlers();
        }

        private void SetupEventHandlers()
        {
            DockingHostViewModel.DockingHostView.LayoutItemsClosing += DockingHostViewLayoutItemsClosing;
            MainWindow.WindowClosing += MainWindow_WindowClosing;
        }

        private async void MainWindow_WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var storableEditors = DockingHostViewModel.Documents.OfType<IStorableEditor>().ToList();
            if (!storableEditors.Any())
                return;
            e.Cancel = await HanldeClose(storableEditors);
        }

        private static async void DockingHostViewLayoutItemsClosing(object sender, Extended.Controls.DockingHost.Views.LayoutItemsClosingEventArgs e)
        {
            var storableEditors = e.LayoutItems.OfType<IStorableEditor>().ToList();
            if (!storableEditors.Any())
                return;
            e.Cancel = await HanldeClose(storableEditors);
        }

        private static async Task<bool> HanldeClose(IEnumerable<IStorableEditor> storableEditors)
        {
            var items = storableEditors.Where(x => x.Document.IsDirty);
            var storableDocuments = items as IList<IStorableEditor> ?? items.ToList();
            if (!storableDocuments.Any())
                return false;

            var saveList = storableDocuments.Select(item => new SaveDirtyDocumentItem(item.Document.FileName)).ToList();
            var result = SaveDirtyDocumentsDialog.Show(saveList);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    foreach (var item in storableDocuments)
                        await item.SaveFile();
                    return false;
                case MessageBoxResult.No:
                    return false;
            }
            return true;
        }
    }
}
