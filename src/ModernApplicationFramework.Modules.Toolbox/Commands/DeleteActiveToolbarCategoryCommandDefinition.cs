using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces.Commands;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;
using ModernApplicationFramework.Modules.Toolbox.Items;

namespace ModernApplicationFramework.Modules.Toolbox.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(DeleteActiveToolbarCategoryCommandDefinition))]
    public class DeleteActiveToolbarCategoryCommandDefinition : CommandDefinition<IDeleteActiveToolbarCategoryCommand>
    {
        public override string NameUnlocalized => "Delete Active";
        public override string Text => NameUnlocalized;
        public override string ToolTip => Text;
        public override Uri IconSource => null;
        public override string IconId => null;
        public override CommandCategory Category => CommandCategories.ToolsCommandCategory;
        public override Guid Id => new Guid("{2A33CF7A-4C10-4FA7-A766-A45F1661F4DF}");
        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures => null;
        public override GestureScope DefaultGestureScope => null;
    }

    public interface IDeleteActiveToolbarCategoryCommand : ICommandDefinitionCommand
    {
    }

    [Export(typeof(IDeleteActiveToolbarCategoryCommand))]
    internal class DeleteActiveToolbarCategoryCommand : CommandDefinitionCommand, IDeleteActiveToolbarCategoryCommand
    {
        private readonly IToolbox _toolbox;
        private readonly IToolboxService _service;

        [ImportingConstructor]
        public DeleteActiveToolbarCategoryCommand(IToolbox toolbox, IToolboxService service)
        {
            _toolbox = toolbox;
            _service = service;
        }

        protected override bool OnCanExecute(object parameter)
        {
            return _toolbox.SelectedNode is IToolboxCategory &&
                   !Equals(_toolbox.SelectedNode, ToolboxCategory.DefaultCategory);
        }

        protected override void OnExecute(object parameter)
        {
            if (!(_toolbox.SelectedNode is IToolboxCategory category))
                return;
            if (category == ToolboxCategory.DefaultCategory)
                return;
            _service.RemoveCategory(category);
        }
    }
}
