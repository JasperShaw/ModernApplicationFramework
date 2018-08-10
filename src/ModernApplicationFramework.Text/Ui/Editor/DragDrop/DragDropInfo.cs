using System.Windows;
using ModernApplicationFramework.Text.Logic;

namespace ModernApplicationFramework.Text.Ui.Editor.DragDrop
{
    public class DragDropInfo
    {
        public Point Location { get; }

        public DragDropKeyStates KeyStates { get; }

        public IDataObject Data { get; }

        public bool IsInternal { get; }

        public object Source { get; }

        public VirtualSnapshotPoint VirtualBufferPosition { get; }

        public DragDropEffects AllowedEffects { get; }

        public DragDropInfo(Point location, DragDropKeyStates keyStates, IDataObject data, bool isInternal, object source, DragDropEffects allowedEffects, VirtualSnapshotPoint bufferPosition)
        {
            Location = location;
            KeyStates = keyStates;
            Data = data;
            IsInternal = isInternal;
            Source = source;
            AllowedEffects = allowedEffects;
            VirtualBufferPosition = bufferPosition;
        }

        public override bool Equals(object obj)
        {
            var dragDropInfo = obj as DragDropInfo;
            if (obj != null && Location == dragDropInfo.Location && (KeyStates == dragDropInfo.KeyStates && Data == dragDropInfo.Data) && (AllowedEffects == dragDropInfo.AllowedEffects && IsInternal == dragDropInfo.IsInternal && Source == dragDropInfo.Source))
                return VirtualBufferPosition == dragDropInfo.VirtualBufferPosition;
            return false;
        }

        public override int GetHashCode()
        {
            return Location.GetHashCode() ^ KeyStates.GetHashCode() ^ Data.GetHashCode() ^ IsInternal.GetHashCode() ^ Source.GetHashCode() ^ AllowedEffects.GetHashCode() ^ VirtualBufferPosition.GetHashCode();
        }

        public static bool operator ==(DragDropInfo first, DragDropInfo second)
        {
            if ((object)first == null)
                return (object)second == null;
            return first.Equals(second);
        }

        public static bool operator !=(DragDropInfo first, DragDropInfo second)
        {
            return !(first == second);
        }
    }
}