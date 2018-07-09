using System;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Imaging
{
    internal struct MonikerAttributeTuple : IEquatable<MonikerAttributeTuple>
    {
        public ImageMoniker Moniker { get; }

        public ImageAttributes Attributes { get; }

        public MonikerAttributeTuple(ImageMoniker moniker, ImageAttributes attributes)
        {
            Moniker = moniker;
            Attributes = attributes;
        }

        public static bool operator ==(MonikerAttributeTuple tuple1, MonikerAttributeTuple tuple2)
        {
            return tuple1.Equals(tuple2);
        }

        public static bool operator !=(MonikerAttributeTuple tuple1, MonikerAttributeTuple tuple2)
        {
            return !tuple1.Equals(tuple2);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is MonikerAttributeTuple))
                return false;
            return Equals((MonikerAttributeTuple)obj);
        }

        public bool Equals(MonikerAttributeTuple other)
        {
            if (Moniker == other.Moniker)
                return Attributes == other.Attributes;
            return false;
        }

        public override int GetHashCode()
        {
            return HashHelpers.CombineHashes(Moniker.GetHashCode(), Attributes.GetHashCode());
        }
    }
}