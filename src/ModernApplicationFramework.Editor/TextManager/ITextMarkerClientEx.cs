using ModernApplicationFramework.Editor.Implementation;

namespace ModernApplicationFramework.Editor.TextManager
{
    public interface ITextMarkerClientEx
    {
        int MarkerInvalidated(IMafTextLines pBuffer, ITextMarker pMarker);

        int OnHoverOverMarker(IMafTextView pView, ITextMarker pMarker, int fShowUi);
    }
}