using System.Collections.Generic;
using ModernApplicationFramework.Controls;
using ModernApplicationFramework.Interfaces.ViewModels;

namespace ModernApplicationFramework.MVVM.Demo
{
    public class MenuCreator : Utilities.MenuCreator
    {
        public override void CreateMenu(IMenuHostViewModel model)
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
    }
}
