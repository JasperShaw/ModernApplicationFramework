using System;
using System.Windows;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Imaging.Interop;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Modules.Toolbox.Interfaces
{
    public interface IToolboxItem : IToolboxNode
    {
        IToolboxCategory Parent { get; set; }

        IToolboxCategory OriginalParent { get; }

        ImageMoniker ImageMoniker { get; }

        IDataObject Data { get; }

        TypeArray<ILayoutItem> CompatibleTypes { get; }

        bool Serializable { get; set; }

        bool IsVisible { get; set; }

        bool IsEnabled { get; set; }

        bool EvaluateEnabled(Type targetType);
    }
}