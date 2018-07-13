using System.Collections.Generic;
using ModernApplicationFramework.Modules.Toolbox.ChooseItemsDialog;

namespace ModernApplicationFramework.Extended.Demo.Modules.CustomToolboxItems
{
    public class DescriptionDataSource : ItemDataSource
    {
        public string Description { get; }

        public DescriptionDataSource(DescriptionToolboxItemDefinition definition) : base(definition)
        {
            Description = definition.Description;
            SearchableStrings = new List<string>{Name, Description};
        }
    }
}