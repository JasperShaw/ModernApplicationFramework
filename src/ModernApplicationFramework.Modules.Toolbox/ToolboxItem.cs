using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;

namespace ModernApplicationFramework.Modules.Toolbox
{
    public class ToolboxItem : ToolboxNodeItem, IToolboxItem
    {
        public IToolboxCategory Parent { get; set; }

        public IToolboxCategory OriginalParent { get; }

        public BitmapSource IconSource { get; set; }

        public TypeArray<ILayoutItem> CompatibleTypes { get; }

        public IDataObject Data { get; }

        public ToolboxItem(Guid id , string name, Type targetType, IToolboxCategory originalParent, IEnumerable<Type> compatibleTypes,  BitmapSource iconSource = null) : 
            this(id, name, null, originalParent, iconSource)
        {
            CompatibleTypes = new TypeArray<ILayoutItem>(compatibleTypes);
            Data = new DataObject(ToolboxItemDataFormats.Type, targetType);
        }

        public ToolboxItem(Guid id , string name, IDataObject data, IToolboxCategory originalParent, BitmapSource iconSource = null) : base(id, name)
        {
            Data = data;
            OriginalParent = originalParent;
            IconSource = iconSource;
        }
    }

}