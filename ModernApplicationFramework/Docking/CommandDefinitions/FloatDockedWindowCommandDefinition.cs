using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Docking.ContextMenuDefinitions;
using ModernApplicationFramework.Docking.Controls;
using ModernApplicationFramework.Docking.Layout;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Docking.CommandDefinitions
{
    [Export(typeof(DefinitionBase))]
    public sealed class FloatDockedWindowCommandDefinition : CommandDefinition
    {
        public FloatDockedWindowCommandDefinition()
        {
            Command = new MultiKeyGestureCommandWrapper(FloatDockedWindow, CanFloatDockedWindow);
        }

        private bool CanFloatDockedWindow()
        {
            var dc = IoC.Get<IContextMenuHost>()
                .GetContextMenu(AnchorableContextMenuDefinition.AnchorableContextMenu)
                .DataContext as LayoutItem;

            return dc?.LayoutElement?.FindParent<LayoutAnchorableFloatingWindow>() == null;
        }

        private void FloatDockedWindow()
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
