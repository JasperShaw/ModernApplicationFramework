using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using ModernApplicationFramework.CommandBase;

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

        public CommandMenuItem(CommandDefinition commandDefinition, ItemsControl parent)
        {
            _parent = parent;
            CommandDefinition = commandDefinition;
            _listItems = new List<ItemsControl>();
            SetValue(VisibilityProperty, Visibility.Collapsed);
        }

        public CommandDefinition CommandDefinition { get; }


        public void Update(CommandHandlerWrapper commandHandler)
        {

            foreach (var listItem in _listItems)
                _parent.Items.Remove(listItem);

            _listItems.Clear();

            

            var listCommands = new List<CommandDefinition>();
            commandHandler.Populate(null, listCommands);

            int startIndex = _parent.Items.IndexOf(this) + 1;

            foreach (var command in listCommands)
            {
                var newMenuItem = CreateItem(command);

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


            //var myBindingC = new Binding
            //{
            //    Source = definition,
            //    Path = new PropertyPath(nameof(definition.Command)),
            //    Mode = BindingMode.OneWay,
            //    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            //};
            //BindingOperations.SetBinding(menuItem, CommandProperty, myBindingC);


            //var c = definition.Command as GestureCommandWrapper;
            //if (c == null)
            //    return menuItem;

            //var myBinding = new Binding
            //{
            //    Source = c,
            //    Path = new PropertyPath(nameof(c.GestureText)),
            //    Mode = BindingMode.OneWay,
            //    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            //};
            //BindingOperations.SetBinding(menuItem, InputGestureTextProperty, myBinding);
            return menuItem;
        }










    }

    public interface ICommandMenuItem
    {
        CommandDefinition CommandDefinition { get; }
        void Update(CommandHandlerWrapper commandHandler);
    }
}