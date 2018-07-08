using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using ModernApplicationFramework.Utilities.Converters;
using ModernApplicationFramework.Utilities.Imaging;

namespace ModernApplicationFramework.Basics.Imaging
{
    [Export(typeof(ImageLibrary))]
    public class ImageLibrary
    {
        private readonly IEnumerable<IImageCatalog> _catalogs;
        private bool _initialized;

        public static readonly Color DefaultGrayscaleBiasColor = Color.FromArgb(64, byte.MaxValue, byte.MaxValue, byte.MaxValue);
        public static readonly Color HighContrastGrayscaleBiasColor = Color.FromArgb(192, byte.MaxValue, byte.MaxValue, byte.MaxValue);

        public bool Initialized
        {
            get => _initialized;
            private set
            {
                if (_initialized == value)
                    return;
                _initialized = value;
                if (!_initialized)
                    return;
                //InitializedChanged.RaiseEvent(this);
            }
        }

        [ImportingConstructor]
        private ImageLibrary([ImportMany] IEnumerable<IImageCatalog> catalogs)
        {
            _catalogs = catalogs;
            Instance = this;
            Initialized = true;
        }

        internal static ImageLibrary Load()
        {
            return IoC.Get<ImageLibrary>();
        }

        public static ImageLibrary Instance { get; internal set; }


        public BitmapSource GetImage(ImageMoniker monkier, ImageAttributes attributes)
        {
            var catalog = _catalogs.FirstOrDefault(x => x.ImageCataloGuid == monkier.CatalogGuid);
            if (catalog == null)
                return null;
            if (!catalog.GetDefinition(monkier.Id, out var imageDefinition))
                return null;


            var visual = Application.LoadComponent(imageDefinition.Source) as FrameworkElement;
            var image = ImageUtilities.FrameworkElementToBitmapSource(visual);

            if (attributes.Background.HasValue)
                image = (BitmapSource)ThemedImageSourceConverter.ConvertCore(image, attributes.Background.Value, attributes.Grayscale,
                    attributes.HighContrast, attributes.GrayscaleBiasColor);
            else if (attributes.Grayscale)
                image = GrayscaleBitmapSourceConverter.ConvertCore(image, attributes.GrayscaleBiasColor);
            else
                image = ImageThemingUtilities.SetOptOutPixel(image);

            return image;
        }
    }

    public interface IImageCatalog
    {
        Guid ImageCataloGuid { get; }

        bool GetDefinition(int id, out ImageDefinition imageDefinition);
    }

    public struct ImageMoniker : IComparable<ImageMoniker>, IEquatable<ImageMoniker>
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
    }

    public struct ImageDefinition
    {
        public ImageMoniker Monkier;
        public Uri Source;
        public ImageType Type;
    }

    public enum ImageType
    {
        Xaml,
        Png
    }
}
