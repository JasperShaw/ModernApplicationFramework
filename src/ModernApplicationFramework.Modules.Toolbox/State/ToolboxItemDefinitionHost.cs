using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.Modules.Toolbox.Items;

namespace ModernApplicationFramework.Modules.Toolbox.State
{

    [Export(typeof(ToolboxItemDefinitionHost))]
    internal class ToolboxItemDefinitionHost
    {
        private readonly IEnumerable<ToolboxItemDefinitionBase> _definitions;
        private readonly IEnumerable<ToolboxCategoryDefinition> _categories;
        private readonly ToolboxItemHost _host;

        public IReadOnlyCollection<ToolboxItemDefinitionBase> Definitions => _definitions.ToList();

        [ImportingConstructor]
        public ToolboxItemDefinitionHost([ImportMany] IEnumerable<ToolboxItemDefinitionBase> definitions,
            [ImportMany] IEnumerable<ToolboxCategoryDefinition> categories, ToolboxItemHost host)
        {
            _definitions = definitions;
            _categories = categories;
            _host = host;

            foreach (var definition in Definitions)
            {
                definition.EnabledChanged += DefinitionOnEnabledChanged;
            }
        }

        private void DefinitionOnEnabledChanged(object sender, EventArgs e)
        {

        }

        public ToolboxItemDefinitionBase GetItemDefinitionById(Guid id)
        {
            if (id == Guid.Empty)
                throw new InvalidOperationException();
            return _definitions.FirstOrDefault(x => x.Id == id);
        }

        public ToolboxCategoryDefinition GetCategoryDefinitionById(Guid id)
        {
            if (id == Guid.Empty)
                throw new InvalidOperationException();
            return _categories.FirstOrDefault(x => x.Id == id);
        }
    }
}
