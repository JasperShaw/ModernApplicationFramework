using System.Windows;
using System.Windows.Media.Imaging;

namespace ModernApplicationFramework.Modules.Toolbox.Interfaces
{
    public interface IToolboxItem : IToolboxNode
    {
        IToolboxCategory Parent { get; set; }

        IToolboxCategory OriginalParent { get; }

        IDataObject Data { get; }

        BitmapSource IconSource { get; set; }
    }
}