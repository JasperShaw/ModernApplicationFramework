﻿using System;
using System.ComponentModel.Composition;
using System.Globalization;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.CommandBar;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
using ModernApplicationFramework.WindowManagement.Interfaces.Commands;
using ModernApplicationFramework.WindowManagement.Properties;

namespace ModernApplicationFramework.WindowManagement.CommandDefinitions
{
    [Export(typeof(CommandBarItemDefinition))]
    [Export(typeof(ResetLayoutCommandDefinition))]
    public sealed class ResetLayoutCommandDefinition : CommandDefinition<IResetLayoutCommand>
    {
        public override string Name => WindowManagement_Resources.ResetLayoutCommandDefinition_Name;
        public override string Text => WindowManagement_Resources.ResetLayoutCommandDefinition_Text;

        public override string NameUnlocalized =>
            WindowManagement_Resources.ResourceManager.GetString("ResetLayoutCommandDefinition_Text",
                CultureInfo.InvariantCulture);

        public override string ToolTip => null;
        public override CommandBarCategory Category => CommandBarCategories.WindowCategory;
        public override Guid Id => new Guid("{A2885FF1-870F-41A3-9259-8A3A2D84286E}");
    }
}
