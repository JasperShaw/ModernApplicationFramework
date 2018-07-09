using System.Windows.Media;
using System.Windows.Media.Imaging;
using ModernApplicationFramework.Modules.Toolbox.Items;

namespace ModernApplicationFramework.Extended.Demo.Modules.FilterDesigner.ViewModels.Elements
{
    [ToolboxItemData("Add", "00000000-0000-0000-0000-000000000000", 0, true, typeof(GraphViewModel))]
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