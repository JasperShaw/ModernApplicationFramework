using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.ContextMenu;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Basics.Definitions.Menu.MenuItems;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Docking.Controls;
using ModernApplicationFramework.Docking.Layout;
using ModernApplicationFramework.Interfaces;


namespace ModernApplicationFramework.Docking.ContextMenuDefinitions
{
    public class AnchorableContextMenuDefinition
    {
        [Export] public static ContextMenuDefinition AnchorableContextMenu = new ContextMenuDefinition(ContextMenuCategory.OtherContextMenusCategory, "Docked Window");

        [Export] public static MenuItemGroupDefinition AnchorableContextMenuGroup = new MenuItemGroupDefinition(AnchorableContextMenu, uint.MinValue);
  
        [Export] public static MenuItemDefinition FloatCommandItemDefinition = new CommandMenuItemDefinition<FloatDockedWindowCommandDefinition>(AnchorableContextMenuGroup, 0)
            ;
        [Export] public static MenuItemDefinition DockCommandItemDefinition = new CommandMenuItemDefinition<DockDockedWindowCommandDefinition>(AnchorableContextMenuGroup, 1);

        [Export] public static MenuItemDefinition HideCommandItemDefinition = new CommandMenuItemDefinition<HideDockedWindowCommandDefinition>(AnchorableContextMenuGroup, 4);
    }

    [Export(typeof(DefinitionBase))]
    public sealed class HideDockedWindowCommandDefinition : CommandDefinition
    {
        public HideDockedWindowCommandDefinition()
        {
            Command = new MultiKeyGestureCommandWrapper(Test, CanTest);
        }

        private bool CanTest()
        {
            var dm = DockingManager.Instace.Layout.ActiveContent;
            return dm is LayoutAnchorable;
        }

        private void Test()
        {
            var dm = DockingManager.Instace.Layout.ActiveContent;
            var item = DockingManager.Instace.GetLayoutItemFromModel(dm) as LayoutAnchorableItem;
            item?.HideCommand.Execute(null);
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

    [Export(typeof(DefinitionBase))]
    public sealed class DockDockedWindowCommandDefinition : CommandDefinition
    {
        public DockDockedWindowCommandDefinition()
        {
            Command = new MultiKeyGestureCommandWrapper(Test, CanTest);
        }

        private bool CanTest()
        {
            var dc = IoC.Get<IContextMenuHost>()
                .GetContextMenu(AnchorableContextMenuDefinition.AnchorableContextMenu)
                .DataContext as LayoutItem;

            return dc?.LayoutElement?.FindParent<LayoutAnchorableFloatingWindow>() != null;
        }

        private void Test()
        {
            var dc = IoC.Get<IContextMenuHost>()
                .GetContextMenu(AnchorableContextMenuDefinition.AnchorableContextMenu)
                .DataContext as LayoutAnchorableItem;

            dc?.DockCommand.Execute(null);
        }

        public override ICommand Command { get; }

        public override string Name => "Dock";
        public override string Text => "Dock";
        public override string ToolTip => null;
        public override Uri IconSource => null;

        public override string IconId => null;
    }

    [Export(typeof(DefinitionBase))]
    public sealed class FloatDockedWindowCommandDefinition : CommandDefinition
    {
        public FloatDockedWindowCommandDefinition()
        {
            Command = new MultiKeyGestureCommandWrapper(Test, CanTest);
        }

        private bool CanTest()
        {
            var dc = IoC.Get<IContextMenuHost>()
                .GetContextMenu(AnchorableContextMenuDefinition.AnchorableContextMenu)
                .DataContext as LayoutItem;

            return dc?.LayoutElement?.FindParent<LayoutAnchorableFloatingWindow>() == null;
        }

        private void Test()
        {
            var dc = IoC.Get<IContextMenuHost>()
                .GetContextMenu(AnchorableContextMenuDefinition.AnchorableContextMenu)
                .DataContext as LayoutAnchorableItem;

            dc?.FloatCommand.Execute(null);
        }

        public override ICommand Command { get; }

        public override string Name => "Float";
        public override string Text => "Float";
        public override string ToolTip => null;
        public override Uri IconSource => null;

        public override string IconId => null;
    }
}
