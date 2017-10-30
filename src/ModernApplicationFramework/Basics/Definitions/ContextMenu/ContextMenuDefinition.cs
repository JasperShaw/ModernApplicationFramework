using System.Globalization;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Core.Converters.AccessKey;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Basics.Definitions.ContextMenu
{
    /// <inheritdoc cref="CommandBarDefinitionBase" />
    /// <summary>
    /// A command bar definition that specifies a context menu
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Basics.Definitions.CommandBar.CommandBarDefinitionBase" />
    /// <seealso cref="T:ModernApplicationFramework.Interfaces.IHasInternalName" />
    public class ContextMenuDefinition : CommandBarDefinitionBase, IHasInternalName
    {
        private string _text;
        private string _internalName;

        /// <summary>
        /// The category of the context menu
        /// </summary>
        public ContextMenuCategory Category { get; }

        /// <summary>
        /// The localized definition's text
        /// </summary>
        /// <inheritdoc />
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

        /// <inheritdoc />
        /// <summary>
        /// The unlocalized internal name of the object
        /// </summary>
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