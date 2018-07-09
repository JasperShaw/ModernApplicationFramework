using System;

namespace ModernApplicationFramework.Imaging
{
    internal struct ImageMoniker : IComparable<ImageMoniker>, IEquatable<ImageMoniker>
    {
        public readonly Guid CatalogGuid;
        public readonly int Id;

        public ImageMoniker(Guid catalogGuid, int id)
        {
            CatalogGuid = catalogGuid;
            Id = id;
        }

        public int CompareTo(ImageMoniker other)
        {
            var num = CatalogGuid.CompareTo(other.CatalogGuid);
            if (num != 0)
                return num;
            return Id - other.Id;
        }

        public bool Equals(ImageMoniker other)
        {
            if (Id == other.Id)
                return CatalogGuid == other.CatalogGuid;
            return false;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ImageMoniker monkier))
                return false;
            return Equals(monkier);
        }

        public override int GetHashCode()
        {
            return CatalogGuid.GetHashCode() ^ Id;
        }

        public static bool operator ==(ImageMoniker monkier1, ImageMoniker monkier2)
        {
            return monkier1.Equals(monkier2);
        }

        public static bool operator !=(ImageMoniker monkier1, ImageMoniker monkier2)
        {
            return !monkier1.Equals(monkier2);
        }

        public Interop.ImageMoniker ToInteropType()
        {
            return new Interop.ImageMoniker
            {
                CatalogGuid = CatalogGuid,
                Id = Id
            };
        }
    }
}