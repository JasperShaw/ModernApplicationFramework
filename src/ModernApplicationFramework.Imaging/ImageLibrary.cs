using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using ModernApplicationFramework.Utilities;
using ModernApplicationFramework.Utilities.Converters;
using ModernApplicationFramework.Utilities.Imaging;

namespace ModernApplicationFramework.Imaging
{
    [Export(typeof(ImageLibrary))]
    public class ImageLibrary
    {
        private readonly IEnumerable<IImageCatalog> _catalogs;
        private bool _initialized;
        private readonly ImageCache _imageCache;

        public static readonly Color DefaultGrayscaleBiasColor = Color.FromArgb(64, byte.MaxValue, byte.MaxValue, byte.MaxValue);
        public static readonly Color HighContrastGrayscaleBiasColor = Color.FromArgb(192, byte.MaxValue, byte.MaxValue, byte.MaxValue);

        public static readonly ImageMoniker EmptyMoniker = new ImageMoniker();

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
            _imageCache = new ImageCache();
        }

        public static ImageLibrary Load()
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

            var cachedImage = _imageCache.GetImage(monkier, attributes);
            if (cachedImage != null)
                return cachedImage;

            var visual = Application.LoadComponent(imageDefinition.Source) as FrameworkElement;
            var image = ImageUtilities.FrameworkElementToBitmapSource(visual);

            if (attributes.Background.HasValue)
                image = (BitmapSource)ThemedImageSourceConverter.ConvertCore(image, attributes.Background.Value, attributes.Grayscale,
                    attributes.HighContrast, attributes.GrayscaleBiasColor);
            else if (attributes.Grayscale)
                image = GrayscaleBitmapSourceConverter.ConvertCore(image, attributes.GrayscaleBiasColor);
            else
                image = ImageThemingUtilities.SetOptOutPixel(image);

            _imageCache.AddImage(image, monkier, attributes);
            return image;
        }
    }

    internal class ImageCache
    {
        private readonly Dictionary<MonikerAttributeTuple, BitmapSource> _store;

        public ImageCache()
        {
            _store = new Dictionary<MonikerAttributeTuple, BitmapSource>();
        }

        public BitmapSource GetImage(ImageMoniker moniker, ImageAttributes attributes)
        {
            var key = MakeKey(moniker, attributes);
            _store.TryGetValue(key, out var image);
            return image;
        }

        public bool AddImage(BitmapSource image, ImageMoniker moniker, ImageAttributes attributes)
        {
            return AddImage(image, moniker, attributes, out _);
        }

        public bool AddImage(BitmapSource image, ImageMoniker moniker, ImageAttributes attributes, out MonikerAttributeTuple key)
        {
            Validate.IsNotNull(image, nameof(image));
            key = MakeKey(moniker, attributes);
            if (_store.ContainsKey(key))
                _store[key] = image;
            else
                _store.Add(key, image);
            return true;
        }

        public bool RemoveImage(ImageMoniker moniker, ImageAttributes attributes)
        {
            var key = MakeKey(moniker, attributes);
            return _store.Remove(key);
        }

        internal static MonikerAttributeTuple MakeKey(ImageMoniker moniker, ImageAttributes attributes)
        {
            return new MonikerAttributeTuple(moniker, attributes);
        }
    }

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
