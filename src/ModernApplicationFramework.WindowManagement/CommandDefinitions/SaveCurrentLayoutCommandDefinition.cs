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
    [Export(typeof(SaveCurrentLayoutCommandDefinition))]
    public sealed class SaveCurrentLayoutCommandDefinition : CommandDefinition<ISaveCurrentLayoutCommand>
    {
        public override string Name => WindowManagement_Resources.SaveLayoutCommandDefinition_Name;
        public override string Text => WindowManagement_Resources.SaveLayoutCommandDefinition_Text;
        public override string NameUnlocalized =>
            WindowManagement_Resources.ResourceManager.GetString("SaveLayoutCommandDefinition_Text",
                CultureInfo.InvariantCulture);
        public override string ToolTip => null;
        public override CommandBarCategory Category => CommandBarCategories.WindowCategory;
        public override Guid Id => new Guid("{046EC243-92CC-4490-83C5-587EB89358DB}");
    }
}
