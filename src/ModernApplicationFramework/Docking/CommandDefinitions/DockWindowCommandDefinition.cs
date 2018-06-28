using System;
using System.Collections.Generic;
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
    [Export(typeof(DockWindowCommandDefinition))]
    public sealed class DockWindowCommandDefinition : CommandDefinition<IDockWindowCommand>
    {
        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures => null;
        public override GestureScope DefaultGestureScope => null;

        public override string Name => Text;

        public override string NameUnlocalized =>
            DockingResources.ResourceManager.GetString("DockWindowCommandDefinition_Text",
                CultureInfo.InvariantCulture);

        public override string Text => DockingResources.DockWindowCommandDefinition_Text;
        public override string ToolTip => null;
        public override Uri IconSource => null;

        public override string IconId => null;

        public override CommandCategory Category => CommandCategories.WindowCommandCategory;
        public override Guid Id => new Guid("{5CC255C6-4D81-4B39-AF49-D80FE4C813EC}");
    }

    public interface IDockWindowCommand : ICommandDefinitionCommand
    {
    }

    [Export(typeof(IDockWindowCommand))]
    internal class DockWindowCommand : CommandDefinitionCommand, IDockWindowCommand
    {
        protected override bool OnCanExecute(object parameter)
        {
            var dc = DockingManager.Instance?.Layout.ActiveContent;
            if (dc == null)
                return false;
            var di = DockingManager.Instance?.GetLayoutItemFromModel(dc);

            return di?.LayoutElement?.FindParent<LayoutFloatingWindow>() != null ||
                   di?.LayoutElement?.FindParent<LayoutDocumentPane>() != null && di.LayoutElement is LayoutAnchorable ||
                   di?.LayoutElement is LayoutAnchorable layoutItem && layoutItem.IsAutoHidden;
        }

        protected override void OnExecute(object parameter)
        {
            var dc = DockingManager.Instance?.Layout.ActiveContent;
            if (dc == null)
                return;
            var di = DockingManager.Instance?.GetLayoutItemFromModel(dc);

            if (di is LayoutAnchorableItem anchorableItem)
                anchorableItem.DockCommand.Execute(null);
            else
                di.DockAsDocumentCommand.Execute(null);
        }
    }
}