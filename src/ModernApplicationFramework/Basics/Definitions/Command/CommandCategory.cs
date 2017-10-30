namespace ModernApplicationFramework.Basics.Definitions.Command
{
    /// <summary>
    /// Localized category description for commands
    /// </summary>
    public class CommandCategory
    {
        /// <summary>
        /// The name of the category
        /// </summary>
        public string Name { get; }

        public string NameUnlocalized { get; }

        public CommandCategory(string name) : this(name, name)
        {
            
        }

        public CommandCategory(string nameUnlocalized, string name)
        {
            NameUnlocalized = nameUnlocalized;
            Name = name;
        }
    }
}