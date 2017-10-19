using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Extended.Commands;

namespace ModernApplicationFramework.MVVM.Demo.Modules.ComboBoxMenuTest.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(TestMenuControllerDefinition))]
    public sealed class TestMenuControllerDefinition : CommandMenuControllerDefinition
    {
        public override string IconId => null;
        public override Uri IconSource => null;

        public override string Name => "MenuController";
        public override string NameUnlocalized => Text;
        public override string Text => "MenuController";
        public override string ToolTip => null;

        public override CommandCategory Category => CommandCategories.EditCommandCategory;

        public TestMenuControllerDefinition()
        {
            Items = new ObservableCollection<CommandBarItemDefinition>
            {
                new CommandBarCommandItemDefinition(int.MaxValue, IoC.Get<UndoCommandDefinition>()),
                new CommandBarCommandItemDefinition(0, IoC.Get<OpenSettingsCommandDefinition>())
            };
        }

        public override ObservableCollection<CommandBarItemDefinition> Items { get; set; }
    }
}