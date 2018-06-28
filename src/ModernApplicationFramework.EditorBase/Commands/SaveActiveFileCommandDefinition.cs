using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.EditorBase.Interfaces.Editor;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Extended.Layout;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.EditorBase.Commands
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

        public override Uri IconSource => new Uri("/ModernApplicationFramework.EditorBase;component/Resources/Icons/Save_16x.xaml",
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

    public interface ISaveActiveFileCommand : ICommandDefinitionCommand
    {
    }

    [Export(typeof(ISaveActiveFileCommand))]
    internal class SaveActiveFileCommand : CommandDefinitionCommand, ISaveActiveFileCommand
    {
        private readonly IDockingHostViewModel _dockingHostViewModel;

        [ImportingConstructor]
        public SaveActiveFileCommand(IDockingHostViewModel dockingHostViewModel)
        {
            _dockingHostViewModel = dockingHostViewModel;
        }
        protected override bool OnCanExecute(object parameter)
        {
            if (_dockingHostViewModel.ActiveItem == null)
                return false;
            return _dockingHostViewModel.ActiveItem is IEditor editor && editor.Document is IStorableFile;
        }

        protected override void OnExecute(object parameter)
        {
            ((IEditor)_dockingHostViewModel.ActiveItem)?.SaveFile();
        }
    }
}
