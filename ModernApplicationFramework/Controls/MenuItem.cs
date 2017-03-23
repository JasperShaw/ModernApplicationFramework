using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ModernApplicationFramework.Basics.Definitions;
using ModernApplicationFramework.CommandBase;
using DefinitionBase = ModernApplicationFramework.Basics.Definitions.DefinitionBase;

namespace ModernApplicationFramework.Controls
{
    public class MenuItem : System.Windows.Controls.MenuItem
    {
        static MenuItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MenuItem), new FrameworkPropertyMetadata(typeof(MenuItem)));
        }
    }

    public class CommandMenuItem : MenuItem, ICommandMenuItem
    {
        private readonly ItemsControl _parent;
        private List<ItemsControl> _listItems;

        public CommandMenuItem(DefinitionBase commandDefinition, ItemsControl parent)
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

            int startIndex = _parent.Items.IndexOf(this) + 1;

            foreach (var command in listCommands)
            {
                var newMenuItem = CreateItem((CommandDefinition)command);

                _parent.Items.Insert(startIndex++, newMenuItem);
                _listItems.Add(newMenuItem);
            }
        }


        private MenuItem CreateItem(CommandDefinition definition)
        {
            object vb = null;
            if (!string.IsNullOrEmpty(definition.IconSource?.OriginalString))
            {
                var myResourceDictionary = new ResourceDictionary { Source = definition.IconSource };
                vb = myResourceDictionary[definition.IconId];
            }
            var menuItem = new MenuItem
            {
                Header = definition.Name,
                Icon = vb
            };
            var myBindingC = new Binding
            {
                Source = definition,
                Path = new PropertyPath(nameof(definition.Command)),
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            BindingOperations.SetBinding(menuItem, CommandProperty, myBindingC);
            var c = definition.Command as GestureCommandWrapper;
            if (c == null)
                return menuItem;

            var myBinding = new Binding
            {
                Source = c,
                Path = new PropertyPath(nameof(c.GestureText)),
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            BindingOperations.SetBinding(menuItem, InputGestureTextProperty, myBinding);
            return menuItem;
        }
    }

    public interface ICommandMenuItem
    {
        DefinitionBase CommandDefinition { get; }
        void Update(CommandHandlerWrapper commandHandler);
    }
}