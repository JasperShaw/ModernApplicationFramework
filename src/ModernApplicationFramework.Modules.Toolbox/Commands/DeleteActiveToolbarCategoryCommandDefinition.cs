using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;
using ModernApplicationFramework.Modules.Toolbox.Items;

namespace ModernApplicationFramework.Modules.Toolbox.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(DeleteActiveToolbarCategoryCommandDefinition))]
    public class DeleteActiveToolbarCategoryCommandDefinition : CommandDefinition
    {
        private readonly IToolbox _toolbox;
        private readonly IToolboxService _service;
        public override string NameUnlocalized => "Delete Active";
        public override string Text => NameUnlocalized;
        public override string ToolTip => Text;
        public override Uri IconSource => null;
        public override string IconId => null;
        public override CommandCategory Category => CommandCategories.ViewCommandCategory;
        public override Guid Id => new Guid("{2A33CF7A-4C10-4FA7-A766-A45F1661F4DF}");
        public override MultiKeyGesture DefaultKeyGesture => null;
        public override GestureScope DefaultGestureScope => null;

        public override ICommand Command { get; }


        [ImportingConstructor]
        public DeleteActiveToolbarCategoryCommandDefinition(IToolbox toolbox, IToolboxService service)
        {
            _toolbox = toolbox;
            _service = service;

            var command = new UICommand(DeleteItem, CanDeleteItem);
            Command = command;
        }

        private bool CanDeleteItem()
        {
            return _toolbox.SelectedNode is IToolboxCategory &&
                   !Equals(_toolbox.SelectedNode, ToolboxCategory.DefaultCategory);
        }

        private void DeleteItem()
        {
            if (!(_toolbox.SelectedNode is IToolboxCategory category))
                return;
            if (category == ToolboxCategory.DefaultCategory)
                return;
            _service.RemoveCategory(category);
        }
    }
}
