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
    [Export(typeof(AddCategoryCommandDefinition))]
    public class AddCategoryCommandDefinition : CommandDefinition
    {
        private readonly IToolboxService _service;

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
        public AddCategoryCommandDefinition(IToolboxService service)
        {
            _service = service;

            var command = new UICommand(AddCategory, CanAddCategory);
            Command = command;
        }

        private static bool CanAddCategory()
        {
            return true;
        }

        private void AddCategory()
        {
            var c = new ToolboxCategory();
            c.CreatedCancelled += C_CreatedCancelled;
            c.Created += C_Created;           
            _service.AddCategory(c);
        }

        private void C_CreatedCancelled(object sender, EventArgs e)
        {
            if (!(sender is IToolboxCategory category))
                return;

            category.Created -= C_Created;
            category.CreatedCancelled -= C_CreatedCancelled;
            _service.RemoveCategory(category);
        }

        private void C_Created(object sender, EventArgs e)
        {
            if (!(sender is IToolboxNode node))
                return;
            node.Created -= C_Created;
            node.CreatedCancelled -= C_CreatedCancelled;
        }
    }
}
