using System;
using System.Globalization;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    public struct CaretPosition
    {
        public CaretPosition(VirtualSnapshotPoint bufferPosition, IMappingPoint mappingPoint, PositionAffinity caretAffinity)
        {
            VirtualBufferPosition = bufferPosition;
            Point = mappingPoint ?? throw new ArgumentNullException(nameof(mappingPoint));
            Affinity = caretAffinity;
        }

        public SnapshotPoint BufferPosition => VirtualBufferPosition.Position;

        public IMappingPoint Point { get; }

        public PositionAffinity Affinity { get; }

        public VirtualSnapshotPoint VirtualBufferPosition { get; }

        public int VirtualSpaces => VirtualBufferPosition.VirtualSpaces;

        public override string ToString()
        {
            if (Affinity == PositionAffinity.Predecessor)
                return string.Format(CultureInfo.InvariantCulture, "|{0}", VirtualBufferPosition);
            return string.Format(CultureInfo.InvariantCulture, "{0}|", VirtualBufferPosition);
        }

        public override int GetHashCode()
        {
            return VirtualBufferPosition.GetHashCode() ^ Affinity.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is CaretPosition position)
                return position == this;
            return false;
        }

        public static bool operator ==(CaretPosition caretPosition1, CaretPosition caretPosition2)
        {
            if (caretPosition1.VirtualBufferPosition == caretPosition2.VirtualBufferPosition)
                return caretPosition1.Affinity == caretPosition2.Affinity;
            return false;
        }

        public static bool operator !=(CaretPosition caretPosition1, CaretPosition caretPosition2)
        {
            return !(caretPosition1 == caretPosition2);
        }
    }
}