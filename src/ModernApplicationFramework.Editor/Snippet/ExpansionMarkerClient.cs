using ModernApplicationFramework.Editor.Implementation;
using ModernApplicationFramework.Editor.TextManager;

namespace ModernApplicationFramework.Editor.Snippet
{
    public class ExpansionMarkerClient : ITextMarkerClient, ITextMarkerClientEx
    {
        //private readonly ExpansionFilter _filter;

        public ExpansionMarkerClient(/*ExpansionFilter filter*/)
        {
            //_filter = filter;
        }

        public int ExecMarkerCommand(ITextMarker pMarker, int iItem)
        {
            return -2147467263;
        }

        public int GetMarkerCommandInfo(ITextMarker pMarker, int iItem, string[] pbstrText, uint[] pcmdf)
        {
            return -2147467263;
        }

        public int GetTipText(ITextMarker pMarker, string[] pbstrText = null)
        {
            //var fieldFromMarker = _filter.GetFieldFromMarker((IVsTextStreamMarker)pMarker);
            //if (fieldFromMarker == null || pbstrText == null || pbstrText.Length != 1)
            return -2147467263;
            //pbstrText[0] = fieldFromMarker.ToolTip;
            //return 0;
        }

        public void MarkerInvalidated()
        {
        }

        public int OnAfterMarkerChange(ITextMarker pMarker)
        {
            return 0;
        }

        public void OnAfterSpanReload()
        {
        }

        public void OnBeforeBufferClose()
        {
        }

        public void OnBufferSave(string pszFileName)
        {
        }

        public int MarkerInvalidated(IMafTextLines pBuffer, ITextMarker pMarker)
        {
            return 0;
        }

        public int OnHoverOverMarker(IMafTextView pView, ITextMarker pMarker, int fShowUi)
        {
            return -2147467263;
        }
    }
}