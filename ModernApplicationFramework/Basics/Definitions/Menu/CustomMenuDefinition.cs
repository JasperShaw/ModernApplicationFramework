namespace ModernApplicationFramework.Basics.Definitions.Menu
{
    public sealed class CustomMenuDefinition : MenuDefinition
    {
        public CustomMenuDefinition(uint sortOrder, string text, string displayName) : base(sortOrder, text, displayName, true)
        {
        }
    }
}
