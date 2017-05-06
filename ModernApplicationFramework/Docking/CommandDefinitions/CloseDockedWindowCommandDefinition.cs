using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.CommandBase;

namespace ModernApplicationFramework.Docking.CommandDefinitions
{
    [Export(typeof(DefinitionBase))]
    [Export(typeof(CloseDockedWindowCommandDefinition))]
    public sealed class CloseDockedWindowCommandDefinition : CommandDefinition
    {
        public override ICommand Command { get; }

        public override string Name => Text;
        public override string Text => DockingResources.CloseDockedWindowCommandDefinition_Text;
        public override string ToolTip => null;
        public override Uri IconSource => null;
        public override string IconId => null;

        public override CommandCategory Category => CommandCategories.FileCommandCategory;

        public CloseDockedWindowCommandDefinition()
        {
            Command = new MultiKeyGestureCommandWrapper(CloseDockedWindow, CanCloseDockedWindow);
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