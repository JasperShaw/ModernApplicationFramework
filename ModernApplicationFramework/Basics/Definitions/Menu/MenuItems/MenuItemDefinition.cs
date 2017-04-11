using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Basics.Definitions.Menu.MenuItems
{
    public class MenuItemDefinition : CommandBarItemDefinition, IHasInternalName
    {
        private MenuItemGroupDefinition _group;
        private string _internalName;

        public MenuItemGroupDefinition Group
        {
            get => _group;
            set
            {
                if (Equals(value, _group)) return;
                _group = value;
                OnPropertyChanged();
            }
        }

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

        public MenuItemDefinition(string name, string text, uint sortOrder, MenuItemGroupDefinition group, DefinitionBase definition, bool visible, bool isChecked,
            bool isCustom, bool isCustomizable)
            : base(text, sortOrder, definition, visible, isChecked, isCustom, isCustomizable)
        {
            _group = group;


            if (group?.Parent is IHasInternalName internalNameParent)
            {
                if (!string.IsNullOrEmpty(internalNameParent.InternalName))
                    _internalName = internalNameParent.InternalName + " | " + name;
            } 
            else
                _internalName = name;
        }
    }
}