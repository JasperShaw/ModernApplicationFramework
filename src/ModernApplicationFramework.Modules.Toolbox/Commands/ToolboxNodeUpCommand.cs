using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;
using ModernApplicationFramework.Modules.Toolbox.Interfaces.Commands;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Modules.Toolbox.Commands
{
    [Export(typeof(IToolboxNodeUpCommand))]
    internal class ToolboxNodeUpCommand : CommandDefinitionCommand, IToolboxNodeUpCommand
    {
        private readonly IToolbox _toolbox;
        private readonly IToolboxService _service;

        [ImportingConstructor]
        public ToolboxNodeUpCommand(IToolbox toolbox, IToolboxService service)
        {
            _toolbox = toolbox;
            _service = service;
        }

        protected override bool OnCanExecute(object parameter)
        {
            if (_toolbox.SelectedNode is IToolboxCategory category)
                return CheckCategoryUp(category);
            if (_toolbox.SelectedNode is IToolboxItem item)
                return CheckItemUp(item);
            return false;
        }

        protected override void OnExecute(object parameter)
        {
            if (_toolbox.SelectedNode is IToolboxCategory category)
                MoveCategoryUp(category);
            if (_toolbox.SelectedNode is IToolboxItem item)
                MoveItemUp(item);
        }

        private bool CheckItemUp(IToolboxItem item)
        {
            if (item.Parent.Items.IndexOf(item) <= 0)
                return false;
            return true;
        }

        private bool CheckCategoryUp(IToolboxCategory category)
        {
            var items = _service.GetToolboxItemSource().ToList();
            if (!items.Contains(category))
                return false;

            var index = items.IndexOf(category);
            if (index <= 0)
                return false;
            if (items.GetRange(0, index).Any(x => x.IsVisible))
                return true;
            return false;
        }

        private void MoveItemUp(IToolboxItem item)
        {
            if (item.Parent == null)
                return;
            var parent = item.Parent;
            var index = item.Parent.Items.IndexOf(item);
            parent.Items.RemoveAt(index);
            parent.Items.Insert(index - 1, item);
            _toolbox.SelectedNode = item;
        }

        private void MoveCategoryUp(IToolboxCategory category)
        {
            if (category == null)
                return;
            var items = _service.GetToolboxItemSource();
            var index = items.IndexOf(x => x.Equals(category));
            _service.RemoveCategory(category, false, true);
            _service.InsertCategory(index - 1, category);
            _toolbox.SelectedNode = category;
        }
    }
}