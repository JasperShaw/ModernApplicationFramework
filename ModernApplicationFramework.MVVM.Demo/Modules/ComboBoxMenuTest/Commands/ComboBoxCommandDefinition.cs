using System;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.MVVM.Demo.Modules.Document;

namespace ModernApplicationFramework.MVVM.Demo.Modules.ComboBoxMenuTest.Commands
{
    [Export(typeof(DefinitionBase))]
    public class ComboBoxCommandDefinition : CommandComboBoxDefinition
    {
        public override string Name => "Combobox";
        public override string Text => Name;
        public override string ToolTip => null;
        public override Uri IconSource => null;
        public override string IconId => null;
        public override CommandCategory Category => CommandCategories.FileCommandCategory;

        public ComboBoxCommandDefinition()
        {
            var itemSource = IoC.Get<SampleViewModel>();
            DataSource = itemSource.DataSource;
        }

        //public override ComboBoxItemsWrapper ItemWrapper { get; }
        public override ComboBoxDataSource DataSource { get; }
    }
}
