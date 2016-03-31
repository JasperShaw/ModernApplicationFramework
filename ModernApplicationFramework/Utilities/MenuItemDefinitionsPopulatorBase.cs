using System.Collections.Generic;

namespace ModernApplicationFramework.Utilities
{
    public abstract class MenuItemDefinitionsPopulatorBase
    {
        protected IList<MenuItemDefinition> MenuDefinitions;

        protected MenuItemDefinitionsPopulatorBase()
        {
            MenuDefinitions = new List<MenuItemDefinition>();
        }

        public abstract IList<MenuItemDefinition> GetDefinitions();
    }
}
