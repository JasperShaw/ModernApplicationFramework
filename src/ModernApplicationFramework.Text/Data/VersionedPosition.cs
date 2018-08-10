using System;
using System.Globalization;

namespace ModernApplicationFramework.Text.Data
{
    public struct VersionedPosition : IEquatable<VersionedPosition>
    {
        public readonly ITextImageVersion Version;
        public readonly int Position;
        public static readonly VersionedPosition Invalid;

        public VersionedPosition(ITextImageVersion version, int position)
        {
            if (version == null)
                throw new ArgumentNullException(nameof(version));
            if (position < 0 || position > version.Length)
                throw new ArgumentOutOfRangeException(nameof(position));
            Version = version;
            Position = position;
        }

        public static implicit operator int(VersionedPosition position)
        {
            return position.Position;
        }

        public VersionedPosition TranslateTo(ITextImageVersion other, PointTrackingMode mode)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));
            return new VersionedPosition(other, other.TrackTo(this, mode));
        }

        public override int GetHashCode()
        {
            if (Version == null)
                return 0;
            return Position ^ Version.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is VersionedPosition position)
                return Equals(position);
            return false;
        }

        public bool Equals(VersionedPosition other)
        {
            if (other.Version == Version)
                return other.Position == Position;
            return false;
        }

        public static bool operator ==(VersionedPosition left, VersionedPosition right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(VersionedPosition left, VersionedPosition right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            if (Version != null)
                return string.Format(CultureInfo.CurrentCulture, "v{0}_{1}", Version.VersionNumber, Position);
            return "Invalid";
        }
    }
}