using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Input;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.ContextMenu;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Basics.Definitions.Menu.MenuItems;
using ModernApplicationFramework.CommandBase;


namespace ModernApplicationFramework.Docking.ContextMenuDefinitions
{
    public class AnchorableContextMenuDefinition
    {
        [Export] public static ContextMenuDefinition AnchorableContextMenu = new ContextMenuDefinition(ContextMenuCategory.OtherContextMenusCategory, "Docked Window");

        [Export] public static MenuItemGroupDefinition AnchorableContextMenuGroup = new MenuItemGroupDefinition(AnchorableContextMenu, uint.MinValue);

        [Export] public static MenuItemDefinition HideCommandItemDefinition = new CommandMenuItemDefinition<HideDockedWindowCommandDefinition>(AnchorableContextMenuGroup, UInt32.MaxValue);
    }

    [Export(typeof(DefinitionBase))]
    public sealed class HideDockedWindowCommandDefinition : CommandDefinition
    {
        public HideDockedWindowCommandDefinition()
        {
            Command = new MultiKeyGestureCommandWrapper(Test, () => true);
        }

        private void Test()
        {
            MessageBox.Show("Test");
        }

        public override ICommand Command { get; }

        public override string Name => "Hide";
        public override string Text => "Hide";
        public override string ToolTip => null;
        public override Uri IconSource =>
            new Uri("/ModernApplicationFramework;component/Resources/Icons/HideToolWindow.xaml",
                UriKind.RelativeOrAbsolute);

        public override string IconId => "HideToolWindow";
    }
}
