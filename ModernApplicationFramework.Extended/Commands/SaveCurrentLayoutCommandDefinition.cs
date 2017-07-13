using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Extended.Core.LayoutUtilities;

namespace ModernApplicationFramework.Extended.Commands
{
    [Export(typeof(DefinitionBase))]
    [Export(typeof(SaveCurrentLayoutCommandDefinition))]
    public sealed class SaveCurrentLayoutCommandDefinition : CommandDefinition
    {
        private readonly ILayoutManager _layoutManager;
        public override string Name => Text;
        public override string Text => "Save Window Layout";
        public override string ToolTip => null;
        public override Uri IconSource => null;
        public override string IconId => null;
        public override CommandCategory Category => CommandCategories.WindowCommandCategory;

        public override ICommand Command { get; }

        [ImportingConstructor]
        public SaveCurrentLayoutCommandDefinition(ILayoutManager layoutManager)
        {
            _layoutManager = layoutManager;
        }
    }
}
