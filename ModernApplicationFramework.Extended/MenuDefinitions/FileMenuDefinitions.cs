﻿using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Basics.Definitions.Menu.MenuItems;
using ModernApplicationFramework.Docking.CommandDefinitions;
using ModernApplicationFramework.Extended.Commands;

namespace ModernApplicationFramework.Extended.MenuDefinitions
{
    public static class FileMenuDefinitions
    {
        [Export] public static MenuDefinition FileMenu =
            new MenuDefinition(MainMenuBarDefinition.MainMenuBar, 0, "File", "&File");

        [Export] public static CommandBarGroupDefinition CloseProgramGroup = new CommandBarGroupDefinition(FileMenu, int.MaxValue);

        [Export] public static CommandBarGroupDefinition CloseLayoutItemGroup = new CommandBarGroupDefinition(FileMenu, 3);

        [Export] public static MenuItemDefinition CloseProgram = new CommandMenuItemDefinition<CloseProgammCommandDefinition>(CloseProgramGroup, 1);

        [Export] public static MenuItemDefinition CloseActiveDocument = new CommandMenuItemDefinition<CloseDockedWindowCommandDefinition>(CloseLayoutItemGroup, 1);
    }
}
