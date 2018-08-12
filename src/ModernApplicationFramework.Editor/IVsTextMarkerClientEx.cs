using ModernApplicationFramework.Editor.Implementation;

namespace ModernApplicationFramework.Editor
{
    public interface IVsTextMarkerClientEx
    {
        int MarkerInvalidated(IMafTextLines pBuffer, IVsTextMarker pMarker);

        int OnHoverOverMarker(IMafTextView pView, IVsTextMarker pMarker, int fShowUi);
    }
}