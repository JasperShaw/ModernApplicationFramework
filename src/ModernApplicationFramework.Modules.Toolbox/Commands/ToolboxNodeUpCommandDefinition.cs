using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Modules.Toolbox.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(ToolboxNodeUpCommandDefinition))]
    public class ToolboxNodeUpCommandDefinition : CommandDefinition
    {
        private readonly IToolbox _toolbox;
        private readonly IToolboxService _service;
        public override string NameUnlocalized => "Move up";
        public override string Text => "Move up";
        public override string ToolTip => Text;
        public override Uri IconSource => null;
        public override string IconId => null;
        public override CommandCategory Category => CommandCategories.EditCommandCategory;
        public override Guid Id => new Guid("{3543E589-5D75-4CF5-88BC-254A14578C69}");
        public override MultiKeyGesture DefaultKeyGesture => null;
        public override GestureScope DefaultGestureScope => null;

        public override ICommand Command { get; }

        [ImportingConstructor]
        public ToolboxNodeUpCommandDefinition(IToolbox toolbox, IToolboxService service)
        {
            _toolbox = toolbox;
            _service = service;
            Command = new UICommand(MoveNodeUp, CanMoveNodeUp);
        }

        private bool CanMoveNodeUp()
        {
            if (_toolbox.SelectedNode is IToolboxCategory category)
                return CheckCategoryUp(category);
            if (_toolbox.SelectedNode is IToolboxItem item)
                return CheckItemUp(item);
            return false;
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

        private void MoveNodeUp()
        {
            if (_toolbox.SelectedNode is IToolboxCategory category)
                MoveCategoryUp(category);
            if (_toolbox.SelectedNode is IToolboxItem item)
                MoveItemUp(item);
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
            _service.RemoveCategory(category, true);
            _service.InsertCategory(index - 1, category);
            _toolbox.SelectedNode = category;
        }
    }
}
