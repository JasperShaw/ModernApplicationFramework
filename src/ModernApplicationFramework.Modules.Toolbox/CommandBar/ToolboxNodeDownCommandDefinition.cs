using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;

namespace ModernApplicationFramework.Modules.Toolbox.CommandBar
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(ToolboxNodeDownCommandDefinition))]
    public class ToolboxNodeDownCommandDefinition : CommandDefinition
    {
        private readonly ToolboxViewModel _toolbox;
        public override string NameUnlocalized => "Move Down";
        public override string Text => "Move Down";
        public override string ToolTip => Text;
        public override Uri IconSource => null;
        public override string IconId => null;
        public override CommandCategory Category => CommandCategories.EditCommandCategory;
        public override Guid Id => new Guid("{FC1C2BD3-A600-4C0D-BE5A-63DE8EED2EA9}");
        public override MultiKeyGesture DefaultKeyGesture => null;
        public override GestureScope DefaultGestureScope => null;

        public override ICommand Command { get; }

        [ImportingConstructor]
        public ToolboxNodeDownCommandDefinition(IToolbox toolbox)
        {
            _toolbox = toolbox as ToolboxViewModel;
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
            if (!_toolbox.Categories.Contains(category))
                return false;
            if (_toolbox.Categories.IndexOf(category) >= _toolbox.Categories.Count - 1)
                return false;
            return true;
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
            var index = _toolbox.Categories.IndexOf(category);
            _toolbox.Categories.RemoveAt(index);
            _toolbox.Categories.Insert(index + 1, category);
            _toolbox.SelectedNode = category;
        }
    }
}
