using System.Globalization;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Core.Converters.AccessKey;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Basics.Definitions.ContextMenu
{
    public class ContextMenuDefinition : CommandBarDefinitionBase, IHasInternalName
    {
        private string _text;
        private string _internalName;
        public ContextMenuCategory Category { get; }

        public override string Text
        {
            get => _text;
            set
            {
                if (value == _text) return;
                _text = value;
                OnPropertyChanged();
            }
        }

        public string InternalName
        {
            get => _internalName;
            set
            {
                if (value == _internalName) return;
                _internalName = value;
                OnPropertyChanged();
            }
        }

        public ContextMenuDefinition(ContextMenuCategory category, string text, bool isCustomizable = true) : base(text,
            uint.MinValue, null, false, isCustomizable, false)
        {
            Category = category;
            _text = $"{category.CategoryName} | {text}";

            _internalName = new AccessKeyRemovingConverter()
                .Convert(_text, typeof(string), null, CultureInfo.CurrentCulture)
                ?.ToString();
        }
    }
}