using System.Windows;
using System.Windows.Data;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.CommandBase;

namespace ModernApplicationFramework.Controls
{
    public class MenuItem : System.Windows.Controls.MenuItem
    {
        static MenuItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MenuItem), new FrameworkPropertyMetadata(typeof(MenuItem)));
        }

        public static MenuItem CreateItem(MenuDefinition definition)
        {
            return new MenuItem { Header = definition.DisplayName };
        }

        /// <summary>
        ///     Creates a MenuItem from CommandDefinition. If the Attached Command is Type GestureCommand the Item will bind the
        ///     Gesture text
        /// </summary>
        /// <param name="definition"></param>
        /// <returns>MenuItem</returns>
        public static MenuItem CreateItemFromDefinition(DefinitionBase definition)
        {
            object vb = null;
            if (!string.IsNullOrEmpty(definition.IconSource?.OriginalString))
            {
                var myResourceDictionary = new ResourceDictionary { Source = definition.IconSource };
                vb = myResourceDictionary[definition.IconId];
            }
            var menuItem = new MenuItem
            {
                Header = definition.Text,
                Icon = vb
            };
            if (!(definition is CommandDefinition commandDefinition))
                return menuItem;
            var myBindingC = new Binding
            {
                Source = definition,
                Path = new PropertyPath(nameof(commandDefinition.Command)),
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            BindingOperations.SetBinding(menuItem, CommandProperty, myBindingC);

            var c = commandDefinition.Command as GestureCommandWrapper;
            if (c == null)
                return menuItem;

            var myBinding = new Binding
            {
                Source = c,
                Path = new PropertyPath(nameof(c.GestureText)),
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            BindingOperations.SetBinding(menuItem, InputGestureTextProperty,
                myBinding);
            if (commandDefinition.IsChecked)
                menuItem.IsChecked = true;
            return menuItem;
        }
    } 
}