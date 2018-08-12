using ModernApplicationFramework.Editor.Implementation;

namespace ModernApplicationFramework.Editor
{
    public class ExpansionMarkerClient : IVsTextMarkerClient, IVsTextMarkerClientEx
    {
        //private readonly ExpansionFilter _filter;

        public ExpansionMarkerClient(/*ExpansionFilter filter*/)
        {
            //_filter = filter;
        }

        public int ExecMarkerCommand(IVsTextMarker pMarker, int iItem)
        {
            return -2147467263;
        }

        public int GetMarkerCommandInfo(IVsTextMarker pMarker, int iItem, string[] pbstrText, uint[] pcmdf)
        {
            return -2147467263;
        }

        public int GetTipText(IVsTextMarker pMarker, string[] pbstrText = null)
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

        public int OnAfterMarkerChange(IVsTextMarker pMarker)
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

        public int MarkerInvalidated(IMafTextLines pBuffer, IVsTextMarker pMarker)
        {
            return 0;
        }

        public int OnHoverOverMarker(IMafTextView pView, IVsTextMarker pMarker, int fShowUi)
        {
            return -2147467263;
        }
    }
}