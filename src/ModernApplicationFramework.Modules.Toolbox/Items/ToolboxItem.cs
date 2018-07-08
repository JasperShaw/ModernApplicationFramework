using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Imaging;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Modules.Toolbox.Items
{
    public class ToolboxItem : ToolboxNode, IToolboxItem
    {
        private bool _isVisible;
        private bool _isEnabled;
        public IToolboxCategory Parent { get; set; }

        public IToolboxCategory OriginalParent { get; }

        public BitmapSource IconSource { get; set; }

        public TypeArray<ILayoutItem> CompatibleTypes { get; }

        public bool Serializable { get; set; }

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (value == _isVisible) return;
                _isVisible = value;
                OnPropertyChanged();
            }
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (value == _isEnabled) return;
                _isEnabled = value;
                OnPropertyChanged();
            }
        }

        public IDataObject Data { get; }

        public ToolboxItem(string name, Type targetType, IEnumerable<Type> compatibleTypes, BitmapSource iconSource = null, bool serializable = true)
            : this(Guid.Empty, name, new DataObject(ToolboxItemDataFormats.Type, targetType), null, compatibleTypes, iconSource, serializable, true)
        {
        }

        public ToolboxItem(string name, IDataObject data, IEnumerable<Type> compatibleTypes, BitmapSource iconSource = null, bool serializable = true)
            : this(Guid.Empty, name, data, null, compatibleTypes, iconSource, serializable, true)
        {
        }

        public ToolboxItem(Guid id, string name, Type targetType, IToolboxCategory originalParent, IEnumerable<Type> compatibleTypes, BitmapSource iconSource = null, bool serializable = true) :
            this(id, name, new DataObject(ToolboxItemDataFormats.Type, targetType), originalParent, compatibleTypes, iconSource, serializable)
        {
        }

        public ToolboxItem(Guid id, string name, IDataObject data, IToolboxCategory originalParent, IEnumerable<Type> compatibleTypes,
            BitmapSource iconSource = null, bool serializable = true, bool isCustom = false) : base(id, name, isCustom)
        {
            Data = data;
            OriginalParent = originalParent;
            IconSource = iconSource;
            CompatibleTypes = new TypeArray<ILayoutItem>(compatibleTypes, true);
            Serializable = serializable;
        }

        internal static IToolboxItem CreateTextItem(IDataObject data)
        {
            //var bitmap = new BitmapImage(
            //    new Uri("pack://application:,,,/ModernApplicationFramework.Modules.Toolbox;component/text.png"));


            var bitmap = ImageLibrary.Instance.GetImage(ImageCatalog.Monikers.Undo, new ImageAttributes());


            string text;
            try
            {
                text = (string)data.GetData(DataFormats.Text);
            }
            catch (ExternalException e)
            {
                return null;
            }

            var dataObject = new DataObject(DataFormats.Text, text);

            var item = new ToolboxItem($"Text: {text}", dataObject, new[] { typeof(object) }, bitmap, true);
            return item;
        }


        public static IToolboxItem CreateCustomItem(Type sourceType)
        {
            var attributes = sourceType?.GetAttributes<ToolboxItemDataAttribute>(false).ToList();
            if (attributes?.FirstOrDefault() == null)
                throw new InvalidOperationException("Item type must have ToolboxItemDataAttribute assigned");
            var attribute = attributes.First();
            return new ToolboxItem(attribute.Name, sourceType, attribute.CompatibleTypes, attribute.IconSource,
                attribute.Serializable);
        }

        public virtual bool EvaluateEnabled(Type targetType)
        {
            return CompatibleTypes.Memebers.Contains(typeof(object)) ||
                   CompatibleTypes.Memebers.Any(targetType.ImplementsOrInharits);
        }
    }

}