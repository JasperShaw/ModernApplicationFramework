using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;

namespace ModernApplicationFramework.Extended.Demo.Modules.ComboBoxMenuTest.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(ComboBoxCommandDefinition))]
    public class ComboBoxCommandDefinition : CommandComboBoxDefinition
    {
        public override string Name => "Combobox";
        public override string NameUnlocalized => Name;
        public override string Text => Name;
        public override string ToolTip => "ToolTip Test";
        public override CommandCategory Category => CommandCategories.FileCommandCategory;
        public override Guid Id => new Guid("{832E08A3-3DEB-4E89-913D-798564087985}");
    }
}
