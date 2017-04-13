using System;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
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

        public MenuDefinition(MenuBarDefinition menuBar, uint sortOrder, string name, string text, bool isCustom = false, bool isCustomizable = true) 
            : base(text, sortOrder, new MenuItemCommandDefinition(), isCustom, isCustomizable, false)
        {
            MenuBar = menuBar;
            _internalName = name;
        }

        private sealed class MenuItemCommandDefinition : DefinitionBase
        {
            public override string Name => null;
            public override string Text => null;
            public override string ToolTip => null;
            public override Uri IconSource => null;
            public override string IconId => null;
            public override bool IsList => false;
            public override CommandCategory Category => null;
            public override string ShortcutText => null;
            public override CommandControlTypes ControlType => CommandControlTypes.Menu;
        }
    }
}