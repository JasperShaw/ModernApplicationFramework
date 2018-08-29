using System;
using System.Collections.Generic;
using ModernApplicationFramework.Docking.Layout;

namespace ModernApplicationFramework.Docking
{
    public class DocumentsClosedEventArgs : EventArgs
    {
        public IEnumerable<LayoutDocument> Documents { get; }

        public DocumentsClosedEventArgs(IEnumerable<LayoutDocument> documents)
        {
            Documents = documents;
        }
    }
}
