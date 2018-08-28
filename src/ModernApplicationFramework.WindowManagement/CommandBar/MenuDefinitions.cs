using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.CommandBar;
using ModernApplicationFramework.Basics.CommandBar.Elements;
using ModernApplicationFramework.WindowManagement.CommandDefinitions;
using ModernApplicationFramework.WindowManagement.Properties;

namespace ModernApplicationFramework.WindowManagement.CommandBar
{
    public static class MenuDefinitions
    {
        //LayoutGroup
        [Export]
        public static CommandBarGroup LayoutGroup =
            new CommandBarGroup(Extended.CommandBarDefinitions.TopLevelMenuDefinitions.WindowMenu, 3);

        [Export]
        public static CommandBarItem SaveLayout =
            new CommandBarCommandItem<SaveCurrentLayoutCommandDefinition>(new Guid("{5070E265-37C1-4D5C-9AED-BC6F0A937189}"), LayoutGroup, 0);

        //------------- Apply Layout Sub Menu
        [Export] public static CommandBarItem ApplyLayout =
            new CommandBarMenuItem(new Guid("{B840B60F-85B0-4A95-B147-09AACF96ACE4}"),
                WindowManagement_Resources.MenuDefinition_ApplyLayout, LayoutGroup, 1);


        [Export]
        public static CommandBarGroup LayoutApplyGroup =
            new CommandBarGroup(ApplyLayout, uint.MinValue);

        [Export]
        public static CommandBarItem ShowLayouts =
            new CommandBarCommandItem<ListWindowLayoutsCommandDefinition>(new Guid("{38D3A38F-03C5-47A5-B226-B2DEC4C1465F}"), LayoutApplyGroup, 0);

        //--------------

        [Export]
        public static CommandBarItem ManageLayouts =
            new CommandBarCommandItem<ManageLayoutCommandDefinition>(new Guid("{7A6A8F10-F147-4AF7-8BB5-1904490A828B}"), LayoutGroup, 2);

        [Export]
        public static CommandBarItem ResetLayout =
            new CommandBarCommandItem<ResetLayoutCommandDefinition>(new Guid("{18B5578C-6C3C-4DCB-858A-DA6DC8E114CB}"), LayoutGroup, 3);
    }
}
