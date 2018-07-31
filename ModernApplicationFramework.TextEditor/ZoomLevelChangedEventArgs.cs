using System;
using System.Windows.Media;

namespace ModernApplicationFramework.TextEditor
{
    public class ZoomLevelChangedEventArgs : EventArgs
    {
        public ZoomLevelChangedEventArgs(double newZoomLevel, Transform transform)
        {
            NewZoomLevel = newZoomLevel;
            ZoomTransform = transform;
        }

        public double NewZoomLevel { get; }

        public Transform ZoomTransform { get; }
    }
}