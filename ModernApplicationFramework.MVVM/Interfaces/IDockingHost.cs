using System;
using System.Collections.Generic;
using System.IO;

namespace ModernApplicationFramework.MVVM.Interfaces
{
    public interface IDockingHost
    {
        void LoadLayout(Stream stream, Action<ITool> addToolCallback, Action<IDocument> addDocumentCallback,
                        Dictionary<string, ILayoutItem> itemsState);

        void SaveLayout(Stream stream);

        void UpdateFloatingWindows();
    }
}