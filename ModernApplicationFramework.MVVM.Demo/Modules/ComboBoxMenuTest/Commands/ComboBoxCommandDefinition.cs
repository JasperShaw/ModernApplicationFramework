using System;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Controls.ComboBox;
using ModernApplicationFramework.MVVM.Demo.Modules.SampleExplorer;

namespace ModernApplicationFramework.MVVM.Demo.Modules.ComboBoxMenuTest.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    public class ComboBoxCommandDefinition : CommandComboBoxDefinition
    {
        public override string Name => "Combobox";
        public override string NameUnlocalized => Name;
        public override string Text => Name;
        public override string ToolTip => "ToolTip Test";
        public override Uri IconSource => null;
        public override string IconId => null;
        public override CommandCategory Category => CommandCategories.FileCommandCategory;

        public ComboBoxCommandDefinition()
        {
            var itemSource = IoC.Get<SampleViewModel>();
            DataSource = itemSource.DataSource;
        }

        public override ComboBoxDataSource DataSource { get; }
    }
}
