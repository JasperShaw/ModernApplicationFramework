using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using ModernApplicationFramework.Imaging.Converters;
using ModernApplicationFramework.Imaging.Interfaces;
using ModernApplicationFramework.Utilities.Imaging;

namespace ModernApplicationFramework.Imaging
{
    [Export(typeof(ImageLibrary))]
    public class ImageLibrary
    {
        private readonly IEnumerable<IImageCatalog> _catalogs;
        private readonly ImageCache _imageCache;

        public static readonly Color DefaultGrayscaleBiasColor = Color.FromArgb(64, byte.MaxValue, byte.MaxValue, byte.MaxValue);
        public static readonly Color HighContrastGrayscaleBiasColor = Color.FromArgb(192, byte.MaxValue, byte.MaxValue, byte.MaxValue);

        public static readonly Interop.ImageMoniker EmptyMoniker = new Interop.ImageMoniker();

        public bool Initialized { get; }

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


        public BitmapSource GetImage(Interop.ImageMoniker monkier, ImageAttributes attributes)
        {
            return GetImage(monkier.ToInternalType(), attributes);
        }

        internal BitmapSource GetImage(ImageMoniker moniker, ImageAttributes attributes)
        {
            var catalog = _catalogs.FirstOrDefault(x => x.ImageCataloGuid == moniker.CatalogGuid);
            if (catalog == null)
                return null;
            if (!catalog.GetDefinition(moniker.Id, out var imageDefinition))
                return null;

            var cachedImage = _imageCache.GetImage(moniker, attributes);
            if (cachedImage != null)
                return cachedImage;


            BitmapSource image;
            try
            {
                
                if (imageDefinition.Type == ImageType.Xaml)
                {
                    var visual = Application.LoadComponent(imageDefinition.Source) as FrameworkElement;
                    image = ImageUtilities.FrameworkElementToBitmapSource(visual);
                }
                else
                    image = new BitmapImage(imageDefinition.Source);
            }
            catch (Exception)
            {
                return null;
            }

            if (attributes.Background.HasValue)
                image = (BitmapSource)ThemedImageSourceConverter.ConvertCore(image, attributes.Background.Value, !attributes.Grayscale,
                    attributes.HighContrast, attributes.GrayscaleBiasColor);
            else if (attributes.Grayscale)
                image = GrayscaleBitmapSourceConverter.ConvertCore(image, attributes.GrayscaleBiasColor);
            else
                image = ImageThemingUtilities.SetOptOutPixel(image);

            _imageCache.AddImage(image, moniker, attributes);
            return image;
        }
    }
}
