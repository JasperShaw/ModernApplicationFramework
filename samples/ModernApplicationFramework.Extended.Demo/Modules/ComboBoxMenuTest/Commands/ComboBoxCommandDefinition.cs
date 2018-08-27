using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.CommandBar.Models;

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
        public override ComboBoxModel Model => TestComboModel.Instance;
    }

    public class TestComboModel : ComboBoxModel
    {
        public static ComboBoxModel Instance = _instance ?? (_instance = new TestComboModel());
        private static ComboBoxModel _instance;

        public TestComboModel()
        {
            Items.Add(new ComboBoxItemModel("123"));
            Items.Add(new ComboBoxItemModel("456"));
            Items.Add(new ComboBoxItemModel("789"));

            //IsEditing = true;
        }
    }
}
