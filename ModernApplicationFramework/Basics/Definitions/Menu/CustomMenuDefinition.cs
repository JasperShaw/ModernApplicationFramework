namespace ModernApplicationFramework.Basics.Definitions.Menu
{
    public sealed class CustomMenuDefinition : MenuDefinition
    {
        public CustomMenuDefinition(MenuBarDefinition menuBar, uint sortOrder, string name, string text) 
            : base(menuBar, sortOrder, name, text, true)
        {
        }
    }
}