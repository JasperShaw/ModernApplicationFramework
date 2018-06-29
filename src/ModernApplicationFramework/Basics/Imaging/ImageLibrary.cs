using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;

namespace ModernApplicationFramework.Basics.Imaging
{
    [Export(typeof(IImageLibrary))]
    internal class ImageLibrary : IImageLibrary
    {
        private readonly IEnumerable<IImageCatalog> _catalogs;

        public ImageLibrary([ImportMany] IEnumerable<IImageCatalog> catalogs)
        {
            _catalogs = catalogs;
        }

        public BitmapSource GetImage(ImageMonkier monkier)
        {
            throw new NotImplementedException();
        }
    }

    public interface IImageLibrary
    {
        BitmapSource GetImage(ImageMonkier monkier);
    }

    public interface IImageCatalog
    {
        Guid ImageCataloGuid { get; }

        bool GetDefinition(int id, out ImageDefinition imageDefinition);
    }

    public struct ImageMonkier : IComparable<ImageMonkier>, IEquatable<ImageMonkier>
    {
        public readonly Guid CatalogGuid;
        public readonly int Id;

        public ImageMonkier(Guid catalogGuid, int id)
        {
            CatalogGuid = catalogGuid;
            Id = id;
        }

        public int CompareTo(ImageMonkier other)
        {
            var num = CatalogGuid.CompareTo(other.CatalogGuid);
            if (num != 0)
                return num;
            return Id - other.Id;
        }

        public bool Equals(ImageMonkier other)
        {
            if (Id == other.Id)
                return CatalogGuid == other.CatalogGuid;
            return false;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ImageMonkier monkier))
                return false;
            return Equals(monkier);
        }

        public override int GetHashCode()
        {
            return CatalogGuid.GetHashCode() ^ Id;
        }

        public static bool operator ==(ImageMonkier monkier1, ImageMonkier monkier2)
        {
            return monkier1.Equals(monkier2);
        }

        public static bool operator !=(ImageMonkier monkier1, ImageMonkier monkier2)
        {
            return !monkier1.Equals(monkier2);
        }
    }

    public struct ImageDefinition
    {
        public ImageMonkier Monkier;
        public Uri Source;
        public ImageType Type;
    }

    public enum ImageType
    {
        Xaml,
        Png
    }
}
