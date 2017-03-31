namespace ModernApplicationFramework.Basics.Definitions.CommandBar
{
    public abstract class CommandBarItemDefinition : CommandBarDefinitionBase
    {
        public abstract bool IsChecked { get; set; }

        private FlagStorage _flagStorage;

        public virtual FlagStorage Flags => _flagStorage ?? (_flagStorage = new FlagStorage());
    }
}
