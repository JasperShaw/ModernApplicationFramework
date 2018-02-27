﻿using System;
using System.ComponentModel.Composition;
using System.Globalization;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Docking.Layout;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Docking.CommandDefinitions
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(DockAsTabbedDocumentCommandDefinition))]
    public sealed class DockAsTabbedDocumentCommandDefinition : CommandDefinition
    {
        public override UICommand Command { get; }

        public override MultiKeyGesture DefaultKeyGesture => null;
        public override GestureScope DefaultGestureScope => null;

        public override string Name => Text;

        public override string NameUnlocalized =>
            DockingResources.ResourceManager.GetString("DockAsTabbedDocumentCommandDefinition_Text",
                CultureInfo.InvariantCulture);

        public override string Text => DockingResources.DockAsTabbedDocumentCommandDefinition_Text;
        public override string ToolTip => null;
        public override Uri IconSource => null;

        public override string IconId => null;

        public override CommandCategory Category => CommandCategories.WindowCommandCategory;
        public override Guid Id => new Guid("{D82DF723-6ED3-43D3-805A-918CC256F6F4}");

        public DockAsTabbedDocumentCommandDefinition()
        {
            Command = new UICommand(DockAsTabbedDocument, CanDockAsTabbedDocument);
        }

        private bool CanDockAsTabbedDocument()
        {
            var dc = DockingManager.Instance.Layout.ActiveContent;
            if (dc == null)
                return false;

            var di = DockingManager.Instance.GetLayoutItemFromModel(dc);
            return di?.LayoutElement != null && di.LayoutElement.FindParent<LayoutAnchorablePane>() != null;
        }

        private void DockAsTabbedDocument()
        {
            var dc = DockingManager.Instance.Layout.ActiveContent;
            if (dc == null)
                return;
            var di = DockingManager.Instance.GetLayoutItemFromModel(dc);
            di?.DockAsDocumentCommand.Execute(null);
        }
    }
}