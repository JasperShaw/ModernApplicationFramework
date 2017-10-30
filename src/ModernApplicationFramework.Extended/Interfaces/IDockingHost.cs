using System;
using System.Collections.Generic;
using System.IO;

namespace ModernApplicationFramework.Extended.Interfaces
{
    public interface IDockingHost
    {
        void LoadLayout(Stream stream, Action<ITool> addToolCallback, Action<ILayoutItem> addDocumentCallback,
            Dictionary<string, ILayoutItemBase> itemsState);

        void SaveLayout(Stream stream);

        void UpdateFloatingWindows();
    }
}