using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Controls.Dialogs;
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

            var command = new MultiKeyGestureCommandWrapper(Save, CanSave);
            Command = command;
        }

        private bool CanSave()
        {
            return true;
        }

        private void Save()
        {
            var c = TextInputDialog.Show("test", "test", "test", out string response);

            MessageBox.Show(c + response);
        }
    }
}
