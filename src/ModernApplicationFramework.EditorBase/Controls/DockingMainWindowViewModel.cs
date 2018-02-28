using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using ModernApplicationFramework.EditorBase.Controls.SaveDirtyDocumentsDialog;
using ModernApplicationFramework.EditorBase.Interfaces.Editor;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;
using ModernApplicationFramework.Extended.Interfaces;

namespace ModernApplicationFramework.EditorBase.Controls
{
    [Export(typeof(IDockingMainWindowViewModel))]
    public class DockingMainWindowViewModel : Extended.Controls.DockingMainWindow.ViewModels.DockingMainWindowViewModel
    {
        protected override void Close()
        {
            var items = DockingHost.Documents.OfType<IStorableEditor>().Where(x => x.Document.IsDirty);
            var storableDocuments = items as IList<IStorableEditor> ?? items.ToList();
            if (!storableDocuments.Any())
            {
                base.Close();
                return;
            }

            var saveList = storableDocuments.Select(item => new SaveDirtyDocumentItem(item.Document.FileName)).ToList();
            var result = SaveDirtyDocumentsDialog.SaveDirtyDocumentsDialog.Show(saveList);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    foreach (var item in storableDocuments)
                        item.SaveFile();
                    base.Close();
                    break;
                case MessageBoxResult.No:
                    base.Close();
                    break;
            }
        }
    }
}