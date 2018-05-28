using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ModernApplicationFramework.Modules.Toolbox
{
    //public class ToolboxItem : ToolboxNodeItem, IToolboxItem
    //{
    //    public Type TargetType { get; }

    //    public ToolboxItemCategory Parent { get; set; }

    //    public ToolboxItemCategory OriginalParent { get; }

    //    public BitmapSource IconSource { get; set; }

    //    public ToolboxItem(Type targetType, ToolboxItemCategory originalParent, string name, BitmapSource iconSource = null) : base(name)
    //    {
    //        TargetType = targetType;
    //        IconSource = iconSource;
    //        OriginalParent = originalParent;
    //    }
    //}

    public interface IToolboxItem
    {
        ToolboxItemCategory Parent { get; set; }

        ToolboxItemCategory OriginalParent { get; }

        IDataObject Data { get; }
    }


    public class ToolboxItemEx : ToolboxNodeItem, IToolboxItem
    {
        public ToolboxItemCategory Parent { get; set; }

        public ToolboxItemCategory OriginalParent { get; }

        public BitmapSource IconSource { get; set; }

        public IDataObject Data { get; }

        public ToolboxItemEx(string name, Type targetType, ToolboxItemCategory originalParent, BitmapSource iconSource = null) : this(name, (IDataObject) null, originalParent, iconSource)
        {
            Data = new DataObject(ToolboxItemDataFormats.Type, targetType);
        }

        public ToolboxItemEx(string name, IDataObject data, ToolboxItemCategory originalParent, BitmapSource iconSource = null) : base(name)
        {
            Data = data;
            OriginalParent = originalParent;
            IconSource = iconSource;
        }
    }

}