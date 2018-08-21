﻿using System;
using System.ComponentModel.Composition;
using System.Globalization;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Interfaces.Commands;

namespace ModernApplicationFramework.Docking.CommandDefinitions
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(AutoHideWindowCommandDefinition))]
    public sealed class AutoHideWindowCommandDefinition : CommandDefinition<IAutoHideWindowCommand>
    {
        public override string Name => Text;

        public override string NameUnlocalized =>
            DockingResources.ResourceManager.GetString("AutoHideWindowCommandDefinition_Text",
                CultureInfo.InvariantCulture);

        public override string Text => DockingResources.AutoHideWindowCommandDefinition_Text;
        public override string ToolTip => null;
        public override CommandCategory Category => CommandCategories.WindowCommandCategory;
        public override Guid Id => new Guid("{2CD1E686-B6D8-4719-875E-9535DE2FF119}");
    }
}