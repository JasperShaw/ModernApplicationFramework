using System;
using System.Windows;
using System.Windows.Media.Imaging;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;

namespace ModernApplicationFramework.Modules.Toolbox
{
    public class ToolboxItemEx : ToolboxNodeItem, IToolboxItem
    {
        public IToolboxCategory Parent { get; set; }

        public IToolboxCategory OriginalParent { get; }

        public BitmapSource IconSource { get; set; }

        public IDataObject Data { get; }

        public ToolboxItemEx(Guid id , string name, Type targetType, IToolboxCategory originalParent, BitmapSource iconSource = null) : 
            this(id, name, (IDataObject) null, originalParent, iconSource)
        {
            Data = new DataObject(ToolboxItemDataFormats.Type, targetType);
        }

        public ToolboxItemEx(Guid id , string name, IDataObject data, IToolboxCategory originalParent, BitmapSource iconSource = null) : base(id, name)
        {
            Data = data;
            OriginalParent = originalParent;
            IconSource = iconSource;
        }
    }

}