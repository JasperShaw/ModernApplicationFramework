using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using ModernApplicationFramework.Commands;
using ModernApplicationFramework.Controls;
using ModernApplicationFramework.Interfaces.Utilities;
using ModernApplicationFramework.Interfaces.ViewModels;

namespace ModernApplicationFramework.MVVM.Demo
{
    public class MenuCreator : IMenuCreator
    {
        public void CreateMenu(IMenuHostViewModel model)
        {
            //Since we can re-build it it would be good to clear it first
            model.Items.Clear();

            var m = new MenuItem { Header = "File" };
            var n = CreateItem(new TestCommandDefinition());

            var a = new MenuItem { Header = "Edit" };
            var b = new MenuItem { Header = "Undo", InputGestureText = "Ctrl + Z" };


            m.Items.Add(n);
            a.Items.Add(b);

            
            var source = new List<MenuItem> { m, a };


            foreach (var menuItem in source)
            {
                model.Items.Add(menuItem);
            }
        }

        private MenuItem CreateItem(CommandDefinition definition)
        {

            var mi = new MenuItem
            {
                Header = definition.Name,
                Command = definition.Command,
                Icon = definition.IconSource,
            };

            var c = definition.Command as GestureCommand;
            if (c == null)
                return mi;

            var myBinding = new Binding
            {
                Source = c,
                Path = new PropertyPath(nameof(c.GestureText)),
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            BindingOperations.SetBinding(mi, System.Windows.Controls.MenuItem.InputGestureTextProperty, myBinding);

            return mi;

        }
    }
}
