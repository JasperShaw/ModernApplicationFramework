namespace ModernApplicationFramework.Basics.CommandBar.ItemDefinitions
{
    /// <summary>
    /// Localized category description for commands
    /// </summary>
    public class CommandBarCategory
    {
        /// <summary>
        /// The name of the category
        /// </summary>
        public string Name { get; }

        public string NameUnlocalized { get; }

        public CommandBarCategory(string name) : this(name, name)
        {
            
        }

        public CommandBarCategory(string nameUnlocalized, string name)
        {
            NameUnlocalized = nameUnlocalized;
            Name = name;
        }
    }
}