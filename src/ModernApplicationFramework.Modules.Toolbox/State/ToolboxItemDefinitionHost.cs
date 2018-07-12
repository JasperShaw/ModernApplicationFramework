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
        private readonly ToolboxItemHost _host;
        public IEnumerable<ToolboxItemDefinitionBase> Definitions { get; }

        [ImportingConstructor]
        public ToolboxItemDefinitionHost([ImportMany] IEnumerable<ToolboxItemDefinitionBase> definitions, ToolboxItemHost host)
        {
            _host = host;
            Definitions = definitions;

            foreach (var definition in Definitions)
            {
                definition.EnabledChanged += DefinitionOnEnabledChanged;
            }
        }

        private void DefinitionOnEnabledChanged(object sender, EventArgs e)
        {

        }

        public ToolboxItemDefinitionBase GetDefinitionById(Guid id)
        {
            if (id == Guid.Empty)
                throw new InvalidOperationException();
            return Definitions.FirstOrDefault(x => x.Id == id);
        }
    }
}
