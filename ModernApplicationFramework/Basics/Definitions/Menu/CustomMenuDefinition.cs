namespace ModernApplicationFramework.Basics.Definitions.Menu
{
    public sealed class CustomMenuDefinition : MenuDefinition
    {
        public CustomMenuDefinition(MenuBarDefinition menuBar,uint sortOrder, string text, string displayName) : base(menuBar, sortOrder, text,
            displayName, true)
        {
        }
    }
}