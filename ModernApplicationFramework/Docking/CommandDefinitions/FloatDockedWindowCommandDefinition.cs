using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Docking.Layout;
using ModernApplicationFramework.Properties;

namespace ModernApplicationFramework.Docking.CommandDefinitions
{
    [Export(typeof(DefinitionBase))]
    [Export(typeof(FloatDockedWindowCommandDefinition))]
    public sealed class FloatDockedWindowCommandDefinition : CommandDefinition
    {
        public override ICommand Command { get; }

        public override string Name => Text;
        public override string Text => Commands_Resources.FloatDockedWindowCommandDefinition_Text;
        public override string ToolTip => null;
        public override Uri IconSource => null;

        public override string IconId => null;

        public override CommandCategory Category => CommandCategories.WindowCommandCategory;

        public FloatDockedWindowCommandDefinition()
        {
            Command = new MultiKeyGestureCommandWrapper(FloatDockedWindow, CanFloatDockedWindow);
        }

        private bool CanFloatDockedWindow()
        {
            var dc = DockingManager.Instace?.Layout.ActiveContent;
            if (dc == null)
                return false;
            var di = DockingManager.Instace?.GetLayoutItemFromModel(dc);
            return di?.LayoutElement?.FindParent<LayoutAnchorableFloatingWindow>() == null;
        }

        private void FloatDockedWindow()
        {
            var dc = DockingManager.Instace?.Layout.ActiveContent;
            if (dc == null)
                return;
            var di = DockingManager.Instace?.GetLayoutItemFromModel(dc);
            di?.FloatCommand.Execute(null);
        }
    }
}