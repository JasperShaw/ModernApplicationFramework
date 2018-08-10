using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.TextEditor.Implementation
{

    //TODO: Add Forward backward stuff
    internal static class ViewLoadedHandler
    {
        //private static List<Tuple<GoBackMarker, int>> _pendingMarkers;

        //internal static void SetTrackingPointOnViewCreation(GoBackMarker marker, int position)
        //{
        //    if (_pendingMarkers == null)
        //        _pendingMarkers = new List<Tuple<GoBackMarker, int>>();
        //    _pendingMarkers.Add(new Tuple<GoBackMarker, int>(marker, position));
        //}

        internal static void OnViewCreated(ITextViewHost host)
        {
            //if (_pendingMarkers == null)
            //    return;
            //var textSnapshot = host.TextView.TextSnapshot;
            //foreach (Tuple<GoBackMarker, int> pendingMarker in _pendingMarkers)
            //    pendingMarker.Item1.SetTrackingPoint(textSnapshot.CreateTrackingPoint(
            //        Math.Min(pendingMarker.Item2, textSnapshot.Length), PointTrackingMode.Positive));
            //_pendingMarkers = null;
        }
    }
}