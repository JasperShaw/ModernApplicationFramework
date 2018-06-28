using System;
using System.Collections.Generic;
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
    [Export(typeof(ToolboxNodeDownCommandDefinition))]
    public class ToolboxNodeDownCommandDefinition : CommandDefinition
    {
        private readonly IToolbox _toolbox;
        private readonly IToolboxService _service;
        public override string NameUnlocalized => "Move Down";

        public override string Name => "Toolbox Down";

        public override string Text => "Move Down";
        public override string ToolTip => Text;
        public override Uri IconSource => null;
        public override string IconId => null;
        public override CommandCategory Category => CommandCategories.ToolsCommandCategory;
        public override Guid Id => new Guid("{FC1C2BD3-A600-4C0D-BE5A-63DE8EED2EA9}");
        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures => null;
        public override GestureScope DefaultGestureScope => null;

        public override ICommand Command { get; }

        [ImportingConstructor]
        public ToolboxNodeDownCommandDefinition(IToolbox toolbox, IToolboxService service)
        {
            _toolbox = toolbox;
            _service = service;
            Command = new UICommand(MoveNodeDown, CanMoveNodeDown);
        }

        private bool CanMoveNodeDown()
        {
            if (_toolbox.SelectedNode is IToolboxCategory category)
                return CheckCategoryDown(category);
            if (_toolbox.SelectedNode is IToolboxItem item)
                return CheckItemDown(item);
            return false;
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

            if (items.GetRange(index +1, items.Count - index -1).Any(x => x.IsVisible))
                return true;

            return false;
        }

        private void MoveNodeDown()
        {
            if (_toolbox.SelectedNode is IToolboxCategory category)
                MoveCategoryDown(category);
            if (_toolbox.SelectedNode is IToolboxItem item)
                MoveItemDown(item);
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
            _service.RemoveCategory(category,false, true);
            _service.InsertCategory(index +1, category);
            _toolbox.SelectedNode = category;
        }
    }
}
