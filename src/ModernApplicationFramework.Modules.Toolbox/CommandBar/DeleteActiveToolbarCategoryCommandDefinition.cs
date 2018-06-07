using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;
using ModernApplicationFramework.Modules.Toolbox.Items;
using ModernApplicationFramework.Modules.Toolbox.State;

namespace ModernApplicationFramework.Modules.Toolbox.CommandBar
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(DeleteActiveToolbarCategoryCommandDefinition))]
    public class DeleteActiveToolbarCategoryCommandDefinition : CommandDefinition
    {
        private readonly ToolboxViewModel _toolbox;
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
        public DeleteActiveToolbarCategoryCommandDefinition(IToolbox toolbox)
        {
            _toolbox = toolbox as ToolboxViewModel;

            var command = new UICommand(DeleteItem, CanDeleteItem);
            Command = command;
        }

        private bool CanDeleteItem()
        {
            return _toolbox.SelectedNode is IToolboxCategory &&
                   _toolbox.SelectedNode != ToolboxItemCategory.DefaultCategory;
        }

        private void DeleteItem()
        {
            if (!(_toolbox.SelectedNode is IToolboxCategory category))
                return;
            if (category == ToolboxItemCategory.DefaultCategory)
                return;
            _toolbox.Categories.Remove(category);
            IoC.Get<ToolboxItemHost>().DeleteNode(category);
        }
    }
}
