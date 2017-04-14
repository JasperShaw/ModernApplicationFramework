using System.Globalization;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Core.Converters.AccessKey;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Basics.Definitions.Menu
{
    public class MenuDefinition : CommandBarDefinitionBase, IHasInternalName
    {
        private string _internalName;
        public MenuBarDefinition MenuBar { get; }

        public virtual string InternalName
        {
            get => _internalName;
            set
            {
                if (value == _internalName) return;
                _internalName = value;
                OnPropertyChanged();
            }
        }

        public MenuDefinition(MenuBarDefinition menuBar, uint sortOrder, string text, bool isCustom = false, bool isCustomizable = true) 
            : base(text, sortOrder, new MenuHeaderCommandDefinition(), isCustom, isCustomizable, false)
        {
            MenuBar = menuBar;
            var accesKeyRemover = new AccessKeyRemovingConverter();
            var convert = accesKeyRemover.Convert(text, typeof(string), null, CultureInfo.CurrentCulture);
            if (convert != null)
                _internalName = convert
                    .ToString();
        }
    }
}