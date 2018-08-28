using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.EditorBase.CommandBar.Resources;
using ModernApplicationFramework.EditorBase.Interfaces.Commands;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Extended.Layout;

namespace ModernApplicationFramework.EditorBase.CommandBar.CommandDefinitions
{
    [Export(typeof(CommandBarItemDefinition))]
    [Export(typeof(SaveActiveFileAsCommandDefinition))]
    public class SaveActiveFileAsCommandDefinition : CommandDefinition<ISaveActiveFileAsCommand>
    {
        private string _text;

        public override string NameUnlocalized => "Save Selected Items As...";
        public override string Text => BuildText();

        private string BuildText()
        {
            if (Command.CanExecute(null))
                return string.Format(CommandsResources.SaveActiveDocumentAsCommandText, _text);
            return CommandsResources.SaveActiveDocumentAsCommandName;
        }

        public override string ToolTip => Text;

        public override string Name => CommandsResources.SaveActiveDocumentAsCommandName;
        public override CommandBarCategory Category => CommandCategories.FileCategory;
        public override Guid Id => new Guid("{651EA782-BFCB-4ACA-8F98-6798C117F988}");

        [ImportingConstructor]
        public SaveActiveFileAsCommandDefinition(IDockingHostViewModel dockingHostViewModel)
        {
            dockingHostViewModel.ActiveLayoutItemChanged += _dockingHostViewModel_ActiveLayoutItemChanged;
        }

        private void _dockingHostViewModel_ActiveLayoutItemChanged(object sender, LayoutChangeEventArgs e)
        {
            _text = e.NewLayoutItem?.DisplayName;
            OnPropertyChanged(nameof(Text));
        }
    }
}
