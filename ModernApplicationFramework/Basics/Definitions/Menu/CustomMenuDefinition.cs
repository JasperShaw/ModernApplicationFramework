namespace ModernApplicationFramework.Basics.Definitions.Menu
{
    public sealed class CustomMenuDefinition : MenuDefinition
    {
        public CustomMenuDefinition(MenuBarDefinition menuBar, uint sortOrder, string text)
            : base(menuBar, sortOrder, text, true)
        {
        }
    }
}