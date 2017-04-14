using ModernApplicationFramework.Basics.Definitions.CommandBar;

namespace ModernApplicationFramework.Basics.Definitions.Menu
{
    public class MenuDefinition : CommandBarItemDefinition
    {
        public MenuDefinition(CommandBarGroupDefinition group ,uint sortOrder, string text, bool isCustom = false,
            bool isCustomizable = true)
            : base(text, sortOrder, group, new MenuHeaderCommandDefinition(), true, false, isCustom, isCustomizable)
        {
        }
    }
}