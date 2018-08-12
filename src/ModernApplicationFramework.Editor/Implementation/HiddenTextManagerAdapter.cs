using ModernApplicationFramework.Editor.TextManager;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Editor.Implementation
{
    internal class HiddenTextManagerAdapter : IHiddenTextManager
    {
        public int GetHiddenTextSession(object pOwningObject, out IHiddenTextSession ppSession)
        {
            ppSession = null;
            var textSessionAdapter = HiddenTextSessionForTextDocData(pOwningObject);
            if (textSessionAdapter == null || !textSessionAdapter.Initialized)
                return -2147418113;
            ppSession = textSessionAdapter;
            return 0;
        }

        public int CreateHiddenTextSession(uint dwFlags, object pOwningObject, IHiddenTextClient pClient, out IHiddenTextSession ppState)
        {
            ppState = null;
            var textSessionAdapter = HiddenTextSessionForTextDocData(pOwningObject);
            if (textSessionAdapter == null || textSessionAdapter.Initialized)
                return -2147418113;
            textSessionAdapter.Init(pClient, dwFlags);
            ppState = textSessionAdapter;
            return 0;
        }

        internal static HiddenTextSessionAdapter HiddenTextSessionForTextDocData(object docDataObject)
        {
            var fromVsTextBuffer = TextDocData.GetDocDataFromMafTextBuffer(docDataObject);
            if (fromVsTextBuffer == null)
                return null;
            var textSessionAdapter = HiddenTextSessionForTextBuffer(fromVsTextBuffer._documentTextBuffer);
            if (textSessionAdapter != null)
            {
                textSessionAdapter.DocData = fromVsTextBuffer;
                textSessionAdapter.Buffer = fromVsTextBuffer._documentTextBuffer;
            }
            return textSessionAdapter;
        }

        internal static HiddenTextSessionAdapter HiddenTextSessionForTextBuffer(ITextBuffer textBuffer)
        {
            return textBuffer?.Properties.GetOrCreateSingletonProperty(() => new HiddenTextSessionAdapter());
        }
    }
}