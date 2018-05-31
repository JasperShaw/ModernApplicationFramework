using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ModernApplicationFramework.Extended.Demo.Modules.FilterDesigner.ViewModels.Elements
{
    public class ImageSource : ElementViewModel
    {
        private BitmapSource _bitmap;
        public BitmapSource Bitmap
        {
            get => _bitmap;
            set
            {
                _bitmap = value;
                NotifyOfPropertyChange(() => PreviewImage);
                RaiseOutputChanged();
            }
        }

        public override BitmapSource PreviewImage => Bitmap;

        public ImageSource()
        {
            SetOutputConnector("Output", Colors.DarkSeaGreen, () => Bitmap);
        }
    }
}