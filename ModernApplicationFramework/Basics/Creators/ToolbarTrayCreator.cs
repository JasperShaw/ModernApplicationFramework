using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.Basics.Definitions.Toolbar;
using ModernApplicationFramework.Interfaces.Command;
using ModernApplicationFramework.Interfaces.Utilities;
using ModernApplicationFramework.Interfaces.ViewModels;

namespace ModernApplicationFramework.Basics.Creators
{
    [Export(typeof(IToolbarTrayCreator))]
    public class ToolbarTrayCreator : IToolbarTrayCreator
    {
        private readonly ToolbarDefinitionOld[] _toolbarDefinitionsOld;

        [ImportingConstructor]
        public ToolbarTrayCreator(ICommandService commandService, [ImportMany] ToolbarDefinitionOld[] toolbarDefinitionsOld)
        {
            _toolbarDefinitionsOld = toolbarDefinitionsOld;
        }

        public void CreateToolbarTray(IToolBarHostViewModel model)
        {
            var definitions = _toolbarDefinitionsOld.OrderBy(x => x.SortOrder);
            foreach (var definition in definitions)
                model.AddToolbarDefinition(definition);
        }
    }
}