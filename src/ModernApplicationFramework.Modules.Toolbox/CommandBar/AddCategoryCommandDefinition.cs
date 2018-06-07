using System;
using System.ComponentModel.Composition;
using System.Linq;
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
    [Export(typeof(AddCategoryCommandDefinition))]
    public class AddCategoryCommandDefinition : CommandDefinition
    {
        private readonly ToolboxViewModel _toolbox;

        public override string NameUnlocalized => "Add Category";
        public override string Text => NameUnlocalized;
        public override string ToolTip => Text;
        public override Uri IconSource => null;
        public override string IconId => null;
        public override CommandCategory Category => CommandCategories.ViewCommandCategory;
        public override Guid Id => new Guid("{D7D3206E-0BBD-41E4-96DF-07EA57571586}");
        public override MultiKeyGesture DefaultKeyGesture => null;
        public override GestureScope DefaultGestureScope => null;

        public override ICommand Command { get; }

        [ImportingConstructor]
        public AddCategoryCommandDefinition(IToolbox toolbox)
        {
            _toolbox = toolbox as ToolboxViewModel;

            var command = new UICommand(RenameItem, CanRenameItem);
            Command = command;
        }

        private static bool CanRenameItem()
        {
            return true;
        }

        private void RenameItem()
        {
            var c = new ToolboxItemCategory();
            c.CreatedCancelled += C_CreatedCancelled;
            c.Created += C_Created;
            
            var index = _toolbox.Categories.Count;
            if (_toolbox.Categories.LastOrDefault() == ToolboxItemCategory.DefaultCategory)
                index--;
            _toolbox.Categories.Insert(index, c);
        }

        private void C_CreatedCancelled(object sender, EventArgs e)
        {
            if (!(sender is IToolboxCategory category))
                return;
            category.Created -= C_Created;
            category.CreatedCancelled -= C_CreatedCancelled;
            _toolbox.Categories.Remove(category);
        }

        private void C_Created(object sender, EventArgs e)
        {
            if (!(sender is IToolboxNode node))
                return;
            node.Created -= C_Created;
            node.CreatedCancelled -= C_CreatedCancelled;
            IoC.Get<ToolboxItemHost>().RegisterNode(node);
        }
    }
}
