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
        public bool Serializable { get; set; }

        public bool IsVisible { get; protected set; }

        public IDataObject Data { get; }

        public ToolboxItem(Guid id , string name, Type targetType, IToolboxCategory originalParent, IEnumerable<Type> compatibleTypes,  BitmapSource iconSource = null, bool serializable = true) : 
            this(id, name, new DataObject(ToolboxItemDataFormats.Type, targetType), originalParent, compatibleTypes, iconSource, serializable)
        {
        }

        public ToolboxItem(Guid id , string name, IDataObject data, IToolboxCategory originalParent, IEnumerable<Type> compatibleTypes, BitmapSource iconSource = null, bool serializable = true) : base(id, name)
        {
            Data = data;
            OriginalParent = originalParent;
            IconSource = iconSource;
            CompatibleTypes = new TypeArray<ILayoutItem>(compatibleTypes);
            Serializable = serializable;
        }

        public void EvaluateVisibility(Type targetType)
        {
            IsVisible = InternalEvaluateVisibility(targetType);
            OnPropertyChanged(nameof(IsVisible));
        }

        protected virtual bool InternalEvaluateVisibility(Type targetType)
        {
            return true;
        }
    }

}