using System.Collections.Generic;
using System.Windows;
using ModernApplicationFramework.MVVM.Core;

namespace ModernApplicationFramework.MVVM.Interfaces
{
    public interface ISaveDirtyDocumentsDialog
    {
        IEnumerable<SaveDirtyDocumentItem> ItemSource { get; }

        MessageBoxResult Result { get; }
    }
}
