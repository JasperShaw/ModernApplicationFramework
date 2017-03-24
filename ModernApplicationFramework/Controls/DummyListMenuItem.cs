using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using ModernApplicationFramework.Basics.Definitions;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Interfaces.Controls;
using DefinitionBase = ModernApplicationFramework.Basics.Definitions.Command.DefinitionBase;

namespace ModernApplicationFramework.Controls
{
    public class DummyListMenuItem : MenuItem, IDummyListMenuItem
    {
        private readonly ItemsControl _parent;
        private readonly List<ItemsControl> _listItems;

        public DummyListMenuItem(DefinitionBase commandDefinition, ItemsControl parent)
        {
            _parent = parent;
            CommandDefinition = commandDefinition;
            _listItems = new List<ItemsControl>();
            SetValue(VisibilityProperty, Visibility.Collapsed);
        }

        public DefinitionBase CommandDefinition { get; }

        public void Update(CommandHandlerWrapper commandHandler)
        {
            foreach (var listItem in _listItems)
                _parent.Items.Remove(listItem);
            _listItems.Clear();
            var listCommands = new List<DefinitionBase>();
            commandHandler.Populate(null, listCommands);
            var startIndex = _parent.Items.IndexOf(this) + 1;
            foreach (var command in listCommands)
            {
                var newMenuItem = CreateItemFromDefinition(command);
                if (command is CommandDefinition commandDefinition && commandDefinition.IsChecked)
                    newMenuItem.IsChecked = true;
                _parent.Items.Insert(startIndex++, newMenuItem);
                _listItems.Add(newMenuItem);
            }
        }
    }
}
