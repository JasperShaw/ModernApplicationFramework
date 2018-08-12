using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Editor
{
    internal class SecondaryTextBufferAdapter : TextBufferAdapter
    {
        public ITextBuffer SecondaryBuffer
        {
            set
            {
                if (MarkerManagerProtected != null)
                {
                    MarkerManagerProtected.BufferClosed();
                    MarkerManagerProtected = null;
                }
                _documentTextBuffer = value;
                _dataTextBuffer = value;
                InitializedDocumentTextBuffer = true;
                OnTextBufferInitialized(null, null);
            }
        }
    }
}