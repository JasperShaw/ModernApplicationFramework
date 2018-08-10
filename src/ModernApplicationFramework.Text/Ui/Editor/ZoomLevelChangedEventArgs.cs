using System;
using System.Windows.Media;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    public class ZoomLevelChangedEventArgs : EventArgs
    {
        public double NewZoomLevel { get; }

        public Transform ZoomTransform { get; }

        public ZoomLevelChangedEventArgs(double newZoomLevel, Transform transform)
        {
            NewZoomLevel = newZoomLevel;
            ZoomTransform = transform;
        }
    }
}