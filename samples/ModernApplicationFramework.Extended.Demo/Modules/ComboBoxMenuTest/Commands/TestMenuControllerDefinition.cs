using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Extended.CommandBar.CommandDefinitions;

namespace ModernApplicationFramework.Extended.Demo.Modules.ComboBoxMenuTest.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(TestMenuControllerDefinition))]
    public sealed class TestMenuControllerDefinition : CommandMenuControllerDefinition
    {
        public override string Name => "MenuController";
        public override string NameUnlocalized => Text;
        public override string Text => "MenuController";
        public override string ToolTip => null;

        public override CommandCategory Category => CommandCategories.EditCommandCategory;
        public override Guid Id => new Guid("{624FF898-788B-476D-B269-879729431260}");

        public TestMenuControllerDefinition()
        {
            Items = new ObservableCollection<CommandBarItemDataSource>
            {
                new CommandBarCommandItem(Guid.Empty, int.MaxValue, IoC.Get<UndoCommandDefinition>()),
                new CommandBarCommandItem(Guid.Empty, 0, IoC.Get<OpenSettingsCommandDefinition>())
            };
        }

        public override ObservableCollection<CommandBarItemDataSource> Items { get; set; }
    }
}