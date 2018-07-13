using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using ModernApplicationFramework.ImageCatalog;
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

        public ToolboxItemDefinitionBase DataSource { get; }

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

        public ToolboxItem(ToolboxItemDefinitionBase dataSource)
            : this(Guid.NewGuid(), null, dataSource, true)
        {
        }

        public ToolboxItem(Guid id, IToolboxCategory originalParent, ToolboxItemDefinitionBase dataSource, bool isCustom = false) 
            : base(id, dataSource.Name, isCustom)
        {
            OriginalParent = originalParent;
            DataSource = dataSource;
        }

        internal static IToolboxItem CreateTextItem(ToolboxItemData data)
        {
            string text;
            try
            {
                text = data.Data.ToString();
            }
            catch (ExternalException)
            {
                return null;
            }

            var dataSource = new ToolboxItemDefinition($"Text: {text}", new ToolboxItemData(DataFormats.Text, text),
                new[] {typeof(object)}, Monikers.Win32Text);

            var item = new ToolboxItem(dataSource);      
            return item;
        }

        public virtual bool EvaluateEnabled(Type targetType)
        {
            if (DataSource != null)
                return DataSource.CompatibleTypes.Memebers.Contains(typeof(object)) ||
                       DataSource.CompatibleTypes.Memebers.Any(targetType.ImplementsOrInharits);
            return false;
        }
    }

}