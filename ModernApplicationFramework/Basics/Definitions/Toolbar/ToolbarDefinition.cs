using System;
using System.Windows.Controls;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using DefinitionBase = ModernApplicationFramework.Basics.Definitions.Command.DefinitionBase;

namespace ModernApplicationFramework.Basics.Definitions.Toolbar
{
    public class ToolbarDefinition : CommandBarDefinitionBase
    {
        private Dock _position;
        private bool _isVisible;


        public Dock Position
        {
            get => _position;
            set
            {
                if (value == _position) return;
                LastPosition = _position;
                _position = value;
                OnPropertyChanged();
            }
        }

        public Dock LastPosition { get; private set; }

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

        public ToolbarDefinition(string text, uint sortOrder, bool visible, Dock position, bool isCustomizable = true,
            bool isCustom = false) : base(text, sortOrder, new ToolbarItemCommandDefinition(), isCustom, isCustomizable,
            false)
        {
            _position = position;
            _isVisible = visible;
        }
    }

    public sealed class ToolbarItemCommandDefinition : DefinitionBase
    {
        public override string Name => null;
        public override string Text => null;
        public override string ToolTip => null;
        public override Uri IconSource => null;
        public override string IconId => null;
        public override bool IsList => false;
        public override CommandCategory Category => null;

        public override CommandControlTypes ControlType => CommandControlTypes.Menu;
        public override string ShortcutText { get; set; }
    }
}