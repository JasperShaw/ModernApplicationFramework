using System.Collections.Generic;
using System.Windows.Media.Imaging;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Imaging
{
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
}