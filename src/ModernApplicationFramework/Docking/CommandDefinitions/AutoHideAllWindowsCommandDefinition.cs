using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Docking.Layout;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces.Commands;

namespace ModernApplicationFramework.Docking.CommandDefinitions
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(AutoHideAllWindowsCommandDefinition))]
    public sealed class AutoHideAllWindowsCommandDefinition : CommandDefinition<IAutoHideAllWindowsCommand>
    {
        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures => null;
        public override GestureScope DefaultGestureScope => null;

        public override string Name => Text;
        public override string NameUnlocalized =>
            DockingResources.ResourceManager.GetString("AutoHideAllWindowsCommandDefinition_Text",
                CultureInfo.InvariantCulture);

        public override string Text => DockingResources.AutoHideAllWindowsCommandDefinition_Text;
        public override string ToolTip => null;
        public override Uri IconSource => null;

        public override string IconId => null;

        public override CommandCategory Category => CommandCategories.WindowCommandCategory;

        public override Guid Id => new Guid("{ABF14EC8-CC6D-4B2A-A1A1-9A0F44690266}");
    }

    public interface IAutoHideAllWindowsCommand : ICommandDefinitionCommand
    {
    }

    [Export(typeof(IAutoHideAllWindowsCommand))]
    internal class AutoHideAllWindowsCommand : CommandDefinitionCommand, IAutoHideAllWindowsCommand
    {
        protected override bool OnCanExecute(object parameter)
        {
            return DockingManager.Instance != null && DockingManager.Instance.Layout.Descendents()
                       .OfType<LayoutAnchorable>().Any(x => !x.IsAutoHidden);
        }

        protected override void OnExecute(object parameter)
        {
            var layoutAnchorables = DockingManager.Instance?.Layout.Descendents().OfType<LayoutAnchorable>();
            if (layoutAnchorables == null)
                return;

            foreach (var anchorable in layoutAnchorables.ToList())
            {
                if (!anchorable.IsAutoHidden)
                    anchorable.ToggleAutoHide();
            }
        }
    }
}