﻿using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Interfaces.Controls;

namespace ModernApplicationFramework.Controls.Menu
{
    public class DummyListMenuItem : MenuItem, IDummyListMenuItem
    {
        private readonly ItemsControl _parent;
        private readonly List<ItemsControl> _listItems;

        public DummyListMenuItem(CommandBarDefinitionBase commandDefinition, ItemsControl parent) : base(commandDefinition)
        {
            _parent = parent;
            CommandBarItemDefinition = commandDefinition;
            _listItems = new List<ItemsControl>();
            SetValue(VisibilityProperty, Visibility.Collapsed);
        }

        public CommandBarDefinitionBase CommandBarItemDefinition { get; }

        public void Update(CommandHandlerWrapper commandHandler)
        {
            foreach (var listItem in _listItems)
                _parent.Items.Remove(listItem);
            _listItems.Clear();
            var listCommands = new List<CommandDefinitionBase>();
            commandHandler.Populate(null, listCommands);
            var startIndex = _parent.Items.IndexOf(this) + 1;
            foreach (var command in listCommands)
            {
                var id = new CommandBarCommandItemDefinition((uint) startIndex ,command);
                var newMenuItem = new MenuItem(id);
                if (command is CommandDefinition commandDefinition && commandDefinition.IsChecked)
                    id.IsChecked = true;
                _parent.Items.Insert(startIndex++, newMenuItem);
                _listItems.Add(newMenuItem);
            }
        }
    }
}