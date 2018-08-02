using ModernApplicationFramework.TextEditor.Implementation;

namespace ModernApplicationFramework.TextEditor
{
    public interface IVsTextMarkerClientEx
    {
        int MarkerInvalidated(IMafTextLines pBuffer, IVsTextMarker pMarker);

        int OnHoverOverMarker(IMafTextView pView, IVsTextMarker pMarker, int fShowUi);
    }
}