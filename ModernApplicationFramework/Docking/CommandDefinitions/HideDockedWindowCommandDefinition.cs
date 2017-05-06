using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Docking.Controls;
using ModernApplicationFramework.Docking.Layout;

namespace ModernApplicationFramework.Docking.CommandDefinitions
{
    [Export(typeof(DefinitionBase))]
    [Export(typeof(HideDockedWindowCommandDefinition))]
    public sealed class HideDockedWindowCommandDefinition : CommandDefinition
    {
        public override ICommand Command { get; }

        public override string Name => Text;
        public override string Text => DockingResources.HideDockedWindowCommandDefinition_Text;
        public override string ToolTip => null;

        public override Uri IconSource =>
            new Uri("/ModernApplicationFramework;component/Resources/Icons/HideToolWindow.xaml",
                UriKind.RelativeOrAbsolute);

        public override string IconId => "HideToolWindow";

        public override CommandCategory Category => CommandCategories.WindowCommandCategory;

        public HideDockedWindowCommandDefinition()
        {
            Command = new MultiKeyGestureCommandWrapper(HideDockedWindow, CanHideDockedWindow);
        }

        private bool CanHideDockedWindow()
        {
            var dm = DockingManager.Instace?.Layout.ActiveContent;
            return dm is LayoutAnchorable;
        }

        private void HideDockedWindow()
        {
            var dm = DockingManager.Instace?.Layout.ActiveContent;
            var item = DockingManager.Instace?.GetLayoutItemFromModel(dm) as LayoutAnchorableItem;
            item?.HideCommand.Execute(null);
        }
    }
}