using System.Collections.Generic;
using System.Windows;
using ModernApplicationFramework.EditorBase.Dialogs.SaveDirtyDocumentsDialog;

namespace ModernApplicationFramework.EditorBase.Interfaces
{
    public interface ISaveDirtyDocumentsDialog
    {
        IEnumerable<SaveDirtyDocumentItem> ItemSource { get; }

        MessageBoxResult Result { get; }
    }
}
