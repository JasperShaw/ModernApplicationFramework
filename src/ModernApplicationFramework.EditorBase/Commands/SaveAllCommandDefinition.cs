using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.EditorBase.Interfaces.Editor;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces.Commands;

namespace ModernApplicationFramework.EditorBase.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(SaveAllCommandDefinition))]
    public class SaveAllCommandDefinition : CommandDefinition<ISaveAllCommand>
    {
        public override string NameUnlocalized => "Save active file";

        public override string Text => CommandsResources.SaveAllCommandText;

        public override string ToolTip => Text;

        public override Uri IconSource => new Uri("/ModernApplicationFramework.EditorBase;component/Resources/Icons/SaveAll_16x.xaml",
            UriKind.RelativeOrAbsolute);
        public override string IconId => "SaveAllFilesIcon";
        public override CommandCategory Category => CommandCategories.FileCommandCategory;
        public override Guid Id => new Guid("{651EA782-BFCB-4ACA-8F98-6798C117F988}");
        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures { get; }
        public override GestureScope DefaultGestureScope { get; }

        public SaveAllCommandDefinition()
        {
            DefaultKeyGestures = new []{ new MultiKeyGesture(Key.S, ModifierKeys.Control | ModifierKeys.Shift)};
            DefaultGestureScope = GestureScopes.GlobalGestureScope;
        }
    }

    public interface ISaveAllCommand : ICommandDefinitionCommand
    {
    }

    [Export(typeof(ISaveAllCommand))]
    internal class SaveAllCommand : CommandDefinitionCommand, ISaveAllCommand
    {
        private readonly IDockingHostViewModel _dockingHostViewModel;

        [ImportingConstructor]
        public SaveAllCommand(IDockingHostViewModel dockingHostViewModel)
        {
            _dockingHostViewModel = dockingHostViewModel;
        }

        protected override bool OnCanExecute(object parameter)
        {
            if (_dockingHostViewModel.LayoutItems == null || _dockingHostViewModel.LayoutItems.Count == 0)
                return false;
            return _dockingHostViewModel.LayoutItems.Any(x => x is IEditor editor && editor.Document is IStorableFile);
        }

        protected override void OnExecute(object parameter)
        {
            foreach (var editor in _dockingHostViewModel.LayoutItems.OfType<IEditor>().Where(x => x.Document is IStorableFile))
            {
                editor.SaveFile();
            }
        }
    }
}
