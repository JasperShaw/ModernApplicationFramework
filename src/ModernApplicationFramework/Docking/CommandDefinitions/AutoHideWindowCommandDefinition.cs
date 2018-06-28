using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Docking.Controls;
using ModernApplicationFramework.Docking.Layout;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Docking.CommandDefinitions
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(AutoHideWindowCommandDefinition))]
    public sealed class AutoHideWindowCommandDefinition : CommandDefinition
    {
        public override ICommand Command { get; }

        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures => null;
        public override GestureScope DefaultGestureScope => null;

        public override string Name => Text;

        public override string NameUnlocalized =>
            DockingResources.ResourceManager.GetString("AutoHideWindowCommandDefinition_Text",
                CultureInfo.InvariantCulture);

        public override string Text => DockingResources.AutoHideWindowCommandDefinition_Text;
        public override string ToolTip => null;
        public override Uri IconSource => null;

        public override string IconId => null;

        public override CommandCategory Category => CommandCategories.WindowCommandCategory;
        public override Guid Id => new Guid("{2CD1E686-B6D8-4719-875E-9535DE2FF119}");

        public AutoHideWindowCommandDefinition()
        {
            Command = new UICommand(AutoHideWindow, CanAutoHideWindow);
        }

        private bool CanAutoHideWindow()
        {
            if (DockingManager.Instance == null)
                return false;
            var dc = DockingManager.Instance.Layout.ActiveContent;
            if (dc == null)
                return false;

            var di = DockingManager.Instance.GetLayoutItemFromModel(dc);

            if (di?.LayoutElement == null)
                return false;

            if (di.LayoutElement.FindParent<LayoutAnchorableFloatingWindow>() != null)
                return false;

            return di.LayoutElement is LayoutAnchorable layoutItem && layoutItem.CanAutoHide &&
                   !layoutItem.IsAutoHidden;
        }

        private void AutoHideWindow()
        {
            var dc = DockingManager.Instance?.Layout.ActiveContent;
            if (dc == null)
                return;

            var di = DockingManager.Instance?.GetLayoutItemFromModel(dc) as LayoutAnchorableItem;

            di?.AutoHideCommand.Execute(null);
        }
    }
}