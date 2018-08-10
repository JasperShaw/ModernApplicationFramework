using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ModernApplicationFramework.Text.Data.Projection
{
    public class GraphBuffersChangedEventArgs : EventArgs
    {
        public ReadOnlyCollection<ITextBuffer> AddedBuffers { get; }

        public ReadOnlyCollection<ITextBuffer> RemovedBuffers { get; }

        public GraphBuffersChangedEventArgs(IList<ITextBuffer> addedBuffers, IList<ITextBuffer> removedBuffers)
        {
            if (addedBuffers == null)
                throw new ArgumentNullException(nameof(addedBuffers));
            if (removedBuffers == null)
                throw new ArgumentNullException(nameof(removedBuffers));
            AddedBuffers = new ReadOnlyCollection<ITextBuffer>(addedBuffers);
            RemovedBuffers = new ReadOnlyCollection<ITextBuffer>(removedBuffers);
        }
    }
}