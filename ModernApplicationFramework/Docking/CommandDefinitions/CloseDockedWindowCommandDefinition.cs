using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.CommandBase;

namespace ModernApplicationFramework.Docking.CommandDefinitions
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(CloseDockedWindowCommandDefinition))]
    public sealed class CloseDockedWindowCommandDefinition : CommandDefinition
    {
        public override UICommand Command { get; }

        public override MultiKeyGesture DefaultKeyGesture => null;
        public override CommandGestureCategory DefaultGestureCategory => null;

        public override string Name => Text;
        public override string Text => DockingResources.CloseDockedWindowCommandDefinition_Text;
        public override string ToolTip => null;
        public override Uri IconSource => null;
        public override string IconId => null;

        public override CommandCategory Category => CommandCategories.FileCommandCategory;

        public CloseDockedWindowCommandDefinition()
        {
            Command = new UICommand(CloseDockedWindow, CanCloseDockedWindow);
        }

        private bool CanCloseDockedWindow()
        {
            var dm = DockingManager.Instace?.Layout.ActiveContent;
            return dm != null;
        }

        private void CloseDockedWindow()
        {
            var dm = DockingManager.Instace?.Layout.ActiveContent;
            var item = DockingManager.Instace?.GetLayoutItemFromModel(dm);
            item?.CloseCommand.Execute(null);
        }
    }
}