﻿using System;
using System.ComponentModel.Composition;
using System.Globalization;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using MordernApplicationFramework.WindowManagement.LayoutManagement;
using MordernApplicationFramework.WindowManagement.Properties;

namespace MordernApplicationFramework.WindowManagement.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(ResetLayoutCommandDefinition))]
    public sealed class ResetLayoutCommandDefinition : CommandDefinition
    {
        private readonly ILayoutManagerInternal _layoutManager;
        private readonly IDefaultWindowLayoutProvider _defaultWindowLayout;
        public override string Name => WindowManagement_Resources.ResetLayoutCommandDefinition_Name;
        public override string Text => WindowManagement_Resources.ResetLayoutCommandDefinition_Text;

        public override string NameUnlocalized =>
            WindowManagement_Resources.ResourceManager.GetString("ResetLayoutCommandDefinition_Text",
                CultureInfo.InvariantCulture);

        public override string ToolTip => null;
        public override Uri IconSource => null;
        public override string IconId => null;
        public override CommandCategory Category => CommandCategories.WindowCommandCategory;

        public override UICommand Command { get; }

        public override MultiKeyGesture DefaultKeyGesture { get; }
        public override GestureScope DefaultGestureScope { get; }

        [ImportingConstructor]
        internal ResetLayoutCommandDefinition(ILayoutManagerInternal layoutManager, IDefaultWindowLayoutProvider defaultWindowLayout)
        {
            _layoutManager = layoutManager;
            _defaultWindowLayout = defaultWindowLayout;

            var command = new UICommand(Manage, CanManage);
            Command = command;
        }

        private bool CanManage()
        {
            return _defaultWindowLayout.GetLayout() != null;
        }

        private void Manage()
        {
            _layoutManager.ApplyWindowLayout(_defaultWindowLayout.GetLayout());
        }
    }
}