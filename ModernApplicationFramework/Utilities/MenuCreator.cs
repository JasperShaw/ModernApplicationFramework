using System.Windows;
using System.Windows.Data;
using ModernApplicationFramework.Commands;
using ModernApplicationFramework.Controls;
using ModernApplicationFramework.Interfaces.Utilities;
using ModernApplicationFramework.Interfaces.ViewModels;

namespace ModernApplicationFramework.Utilities
{
    public abstract class MenuCreator : IMenuCreator
    {
        /// <summary>
        /// Create a Menu and add it to the ViewModel
        /// </summary>
        /// <param name="model"></param>
        public abstract void CreateMenu(IMenuHostViewModel model);

        /// <summary>
        /// Creates a MenuItem from CommandDefinition. If the Attached Command is Type GestureCommand the Item will bind the Gesture text
        /// </summary>
        /// <param name="definition"></param>
        /// <returns></returns>
        protected virtual MenuItem CreateItem(CommandDefinition definition)
        {
            var menuItem = new MenuItem
            {
                Header = definition.Name,
                Command = definition.Command,
                Icon = definition.IconSource,
            };

            var c = definition.Command as GestureCommand;
            if (c == null)
                return menuItem;

            var myBinding = new Binding
            {
                Source = c,
                Path = new PropertyPath(nameof(c.GestureText)),
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            BindingOperations.SetBinding(menuItem, System.Windows.Controls.MenuItem.InputGestureTextProperty, myBinding);
            return menuItem;
        }
    }
}
