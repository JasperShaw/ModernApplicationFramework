using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;
using ModernApplicationFramework.Modules.Toolbox.Interfaces.Commands;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Modules.Toolbox.Commands
{
    [Export(typeof(IToolboxNodeDownCommand))]
    internal class ToolboxNodeDownCommand : CommandDefinitionCommand, IToolboxNodeDownCommand
    {
        private readonly IToolbox _toolbox;
        private readonly IToolboxService _service;

        [ImportingConstructor]
        public ToolboxNodeDownCommand(IToolbox toolbox, IToolboxService service)
        {
            _toolbox = toolbox;
            _service = service;
        }

        protected override bool OnCanExecute(object parameter)
        {
            if (_toolbox.SelectedNode is IToolboxCategory category)
                return CheckCategoryDown(category);
            if (_toolbox.SelectedNode is IToolboxItem item)
                return CheckItemDown(item);
            return false;
        }
        protected override void OnExecute(object parameter)
        {
            if (_toolbox.SelectedNode is IToolboxCategory category)
                MoveCategoryDown(category);
            if (_toolbox.SelectedNode is IToolboxItem item)
                MoveItemDown(item);
        }

        private bool CheckItemDown(IToolboxItem item)
        {
            if (item.Parent.Items.IndexOf(item) >= item.Parent.Items.Count - 1)
                return false;
            return true;
        }

        private bool CheckCategoryDown(IToolboxCategory category)
        {
            var items = _service.GetToolboxItemSource().ToList();

            if (!items.Contains(category))
                return false;

            var index = items.IndexOf(category);
            if (index >= items.Count - 1)
                return false;

            if (items.GetRange(index + 1, items.Count - index - 1).Any(x => x.IsVisible))
                return true;

            return false;
        }

        private void MoveItemDown(IToolboxItem item)
        {
            if (item.Parent == null)
                return;
            var parent = item.Parent;
            var index = item.Parent.Items.IndexOf(item);
            parent.Items.RemoveAt(index);
            parent.Items.Insert(index + 1, item);
            _toolbox.SelectedNode = item;
        }

        private void MoveCategoryDown(IToolboxCategory category)
        {
            if (category == null)
                return;
            var items = _service.GetToolboxItemSource();
            var index = items.IndexOf(x => x.Equals(category));
            _service.RemoveCategory(category, false, true);
            _service.InsertCategory(index + 1, category);
            _toolbox.SelectedNode = category;
        }
    }
}