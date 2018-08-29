using System.Collections.Generic;
using System.ComponentModel;
using ModernApplicationFramework.Docking.Layout;

namespace ModernApplicationFramework.Docking
{
    public class DocumentsClosingEventArgs : CancelEventArgs
    {
        public IEnumerable<LayoutDocument> Documents { get; }

        public DocumentsClosingEventArgs(IEnumerable<LayoutDocument> documents)
        {
            Documents = documents;
        }
    }
}
