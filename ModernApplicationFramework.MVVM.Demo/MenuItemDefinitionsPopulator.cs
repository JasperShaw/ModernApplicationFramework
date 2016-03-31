using System.Collections.Generic;
using ModernApplicationFramework.Commands;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.MVVM.Demo
{
    public class MenuItemDefinitionsPopulator : MenuItemDefinitionsPopulatorBase
    {
        public override IList<MenuItemDefinition> GetDefinitions()
        {
            var fileMenu = new MenuItemDefinition("File", 0);

            var subMenu = new MenuItemDefinition("Sub", 0, fileMenu);
            var subtestMenu1 = new MenuItemDefinition("Test", 0, subMenu,
                new List<CommandDefinition> {new TestCommandDefinition()});
            var subtestMenu2 = new MenuItemDefinition("Test", 0, subMenu,
                new List<CommandDefinition> {new TestCommandDefinition()});

            var subsubMenu = new MenuItemDefinition("Sub", 0, subMenu);
            var subsubtestMenu1 = new MenuItemDefinition("Test", 0, subsubMenu,
                new List<CommandDefinition> {new TestCommandDefinition()});

            var testMenu1 = new MenuItemDefinition("Test", 0, fileMenu,
                new List<CommandDefinition> {new TestCommandDefinition()});
            var testMenu2 = new MenuItemDefinition("Test", 0, fileMenu,
                new List<CommandDefinition> {new TestCommandDefinition()});
            var testMenu3 = new MenuItemDefinition("Test", 0, fileMenu,
                new List<CommandDefinition> {new TestCommandDefinition()});

            var editMenu = new MenuItemDefinition("Edit", 0);
            var testMenu4 = new MenuItemDefinition("Test", 0, editMenu,
                new List<CommandDefinition> {new TestCommandDefinition()});

            MenuDefinitions.Add(fileMenu);

            MenuDefinitions.Add(subMenu);

            MenuDefinitions.Add(testMenu1);
            MenuDefinitions.Add(testMenu2);
            MenuDefinitions.Add(testMenu3);


            MenuDefinitions.Add(subtestMenu1);
            MenuDefinitions.Add(subtestMenu2);
            MenuDefinitions.Add(subtestMenu2);

            MenuDefinitions.Add(subsubMenu);
            MenuDefinitions.Add(subsubtestMenu1);

            MenuDefinitions.Add(editMenu);
            MenuDefinitions.Add(testMenu4);


            return MenuDefinitions;
        }
    }
}
