using System;
using System.ComponentModel.Composition;
using System.Globalization;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Docking.Controls;
using ModernApplicationFramework.Docking.Layout;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Docking.CommandDefinitions
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(HideDockedWindowCommandDefinition))]
    public sealed class HideDockedWindowCommandDefinition : CommandDefinition
    {
        public override UICommand Command { get; }

        public override MultiKeyGesture DefaultKeyGesture => null;
        public override GestureScope DefaultGestureScope => null;

        public override string Name => Text;

        public override string NameUnlocalized =>
            DockingResources.ResourceManager.GetString("HideDockedWindowCommandDefinition_Text",
                CultureInfo.InvariantCulture);

        public override string Text => DockingResources.HideDockedWindowCommandDefinition_Text;
        public override string ToolTip => null;

        public override Uri IconSource =>
            new Uri("/ModernApplicationFramework;component/Resources/Icons/HideToolWindow.xaml",
                UriKind.RelativeOrAbsolute);

        public override string IconId => "HideToolWindow";

        public override CommandCategory Category => CommandCategories.WindowCommandCategory;

        public HideDockedWindowCommandDefinition()
        {
            Command = new UICommand(HideDockedWindow, CanHideDockedWindow);
        }

        private bool CanHideDockedWindow()
        {
            var dm = DockingManager.Instance?.Layout.ActiveContent;
            return dm is LayoutAnchorable;
        }

        private void HideDockedWindow()
        {
            var dm = DockingManager.Instance?.Layout.ActiveContent;
            var item = DockingManager.Instance?.GetLayoutItemFromModel(dm) as LayoutAnchorableItem;
            item?.HideCommand.Execute(null);
        }
    }
}