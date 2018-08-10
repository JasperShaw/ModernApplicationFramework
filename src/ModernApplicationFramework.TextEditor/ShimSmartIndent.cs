using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.TextEditor.Text;

namespace ModernApplicationFramework.TextEditor
{
    internal sealed class ShimSmartIndent : ISmartIndent
    {
        internal bool Connected;
        internal ITextView TextView;

        public ShimSmartIndent(ITextView textView)
        {
            TextView = textView;
        }

        public void Dispose()
        {
            Disconnect();
            TextView = null;
        }

        private void Disconnect()
        {
            Connected = false;
        }

        private void Connect()
        {
            Connected = true;
        }

        public int? GetDesiredIndentation(ITextSnapshotLine line)
        {
            if (TextView != null)
            {
                if (!Connected)
                    Connect();
            }
            return new int?();
        }
    }
}