using ModernApplicationFramework.Basics.Definitions.CommandBar;

namespace ModernApplicationFramework.Basics.Definitions.ContextMenu
{
    public class ContextMenuDefinition : CommandBarDefinitionBase
    {
        private string _text;
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

        public ContextMenuDefinition(ContextMenuCategory category, string text, bool isCustomizable = true) : base(text, uint.MinValue, null, false, isCustomizable, false)
        {
            Category = category;
            _text = $"{category.CategoryName} | {text}";
        }
    }
}
