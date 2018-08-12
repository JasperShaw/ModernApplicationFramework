using ModernApplicationFramework.Editor.Implementation;

namespace ModernApplicationFramework.Editor.TextManager
{
    public interface IMafTextManager
    {
        int GetMarkerTypeInterface(int markerTypeId, out IVsTextMarkerType ppMarkerType);
    }
}