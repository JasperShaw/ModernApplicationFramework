using System.Collections.Generic;

namespace ModernApplicationFramework.Utilities
{
    public abstract class ToolbarDefinitionsPopulatorBase
    {
        protected IList<ToolbarDefinition> ToolbarDefinition;

        protected ToolbarDefinitionsPopulatorBase()
        {
            ToolbarDefinition = new List<ToolbarDefinition>();
        }

        public abstract IList<ToolbarDefinition> GetDefinitions();
    }
}
