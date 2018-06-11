using System;
using System.Windows;
using System.Windows.Media.Imaging;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Modules.Toolbox.Interfaces
{
    public interface IToolboxItem : IToolboxNode
    {
        IToolboxCategory Parent { get; set; }

        IToolboxCategory OriginalParent { get; }

        IDataObject Data { get; }

        BitmapSource IconSource { get; set; }

        TypeArray<ILayoutItem> CompatibleTypes { get; }

        bool Serializable { get; set; }

        bool IsVisible { get; set; }

        bool IsEnabled { get; set; }

        bool EvaluateEnabled(Type targetType);
    }
}