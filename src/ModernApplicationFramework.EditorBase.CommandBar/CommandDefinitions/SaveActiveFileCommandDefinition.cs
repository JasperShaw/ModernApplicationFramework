using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.EditorBase.CommandBar.Resources;
using ModernApplicationFramework.EditorBase.Interfaces.Commands;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Extended.Layout;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.EditorBase.CommandBar.CommandDefinitions
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(SaveActiveFileCommandDefinition))]
    public class SaveActiveFileCommandDefinition : CommandDefinition<ISaveActiveFileCommand>
    {
        private string _text;

        public override string NameUnlocalized => "Save active file";
        public override string Text => BuildText();

        private string BuildText()
        {
            if (Command.CanExecute(null))
                return string.Format(CommandsResources.SaveActiveDocumentCommandText, _text);
            return CommandsResources.SaveActiveDocumentCommandName;
        }

        public override string ToolTip => Text;

        public override string Name => CommandsResources.SaveActiveDocumentCommandName;

        public override Uri IconSource => new Uri("/ModernApplicationFramework.EditorBase.CommandBar;component/Resources/Icons/Save_16x.xaml",
            UriKind.RelativeOrAbsolute);
        public override string IconId => "SaveFileIcon";
        public override CommandCategory Category => CommandCategories.FileCommandCategory;
        public override Guid Id => new Guid("{651EA782-BFCB-4ACA-8F98-6798C117F988}");
        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures { get; }
        public override GestureScope DefaultGestureScope { get; }

        [ImportingConstructor]
        public SaveActiveFileCommandDefinition(IDockingHostViewModel dockingHostViewModel)
        {
            DefaultKeyGestures = new []{ new MultiKeyGesture(Key.S, ModifierKeys.Control)};
            DefaultGestureScope = GestureScopes.GlobalGestureScope;

            dockingHostViewModel.ActiveLayoutItemChanged += _dockingHostViewModel_ActiveLayoutItemChanged;
        }

        private void _dockingHostViewModel_ActiveLayoutItemChanged(object sender, LayoutChangeEventArgs e)
        {
            _text = e.NewLayoutItem?.DisplayName;
            OnPropertyChanged(nameof(Text));
        }
    }
}
