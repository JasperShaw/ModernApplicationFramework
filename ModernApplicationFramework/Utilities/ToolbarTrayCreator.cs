using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.CommandBase.Service;
using ModernApplicationFramework.Interfaces.Utilities;
using ModernApplicationFramework.Interfaces.ViewModels;

namespace ModernApplicationFramework.Utilities
{
    [Export(typeof(IToolbarTrayCreator))]
    public class ToolbarTrayCreator : IToolbarTrayCreator
    {
        private readonly ToolbarDefinition[] _toolbarDefinitions;

        [ImportingConstructor]
        public ToolbarTrayCreator(ICommandService commandService, [ImportMany] ToolbarDefinition[] toolbarDefinitions)
        {
            _toolbarDefinitions = toolbarDefinitions;
        }

        public void CreateToolbarTray(IToolBarHostViewModel model)
        {
            var definitions = _toolbarDefinitions.OrderBy(x => x.SortOrder);
            foreach (var definition in definitions)
                model.AddToolbarDefinition(definition);
        }
    }
}