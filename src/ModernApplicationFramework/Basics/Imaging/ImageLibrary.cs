using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using System.Xml;
using Caliburn.Micro;
using ModernApplicationFramework.Utilities.Imaging;

namespace ModernApplicationFramework.Basics.Imaging
{
    [Export(typeof(ImageLibrary))]
    public class ImageLibrary
    {
        private readonly IEnumerable<IImageCatalog> _catalogs;
        private bool _initialized;

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


        public BitmapSource GetImage(ImageMoniker monkier)
        {
            var catalog = _catalogs.FirstOrDefault(x => x.ImageCataloGuid == monkier.CatalogGuid);
            if (catalog == null)
                return null;
            if (!catalog.GetDefinition(monkier.Id, out var imageDefinition))
                return null;


            FrameworkElement visual;
            //using (var stream = imageDefinition.GetType().Assembly.GetManifestResourceStream(imageDefinition.Source.LocalPath))
            using (var stream = Application.GetResourceStream(imageDefinition.Source).Stream)
            {
                using (var stringReader = new StreamReader(stream))
                {
                    using (var xmlReader = new XmlTextReader(stringReader))
                    {
                        visual = (FrameworkElement)XamlReader.Load(xmlReader);
                    }
                }
            }
            return ImageUtilities.FrameworkElementToBitmapSource(visual);
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
