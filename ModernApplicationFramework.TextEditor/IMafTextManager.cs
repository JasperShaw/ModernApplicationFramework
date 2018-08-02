namespace ModernApplicationFramework.TextEditor
{
    public interface IMafTextManager
    {
        int GetMarkerTypeInterface(int markerTypeId, out IVsTextMarkerType ppMarkerType);
    }

    internal class TextManager : IMafTextManager
    {
        public int GetMarkerTypeInterface(int markerTypeId, out IVsTextMarkerType ppMarkerType)
        {
            throw new System.NotImplementedException();
        }
    }
}