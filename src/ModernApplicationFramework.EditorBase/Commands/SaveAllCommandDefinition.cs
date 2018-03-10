using System;
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

namespace ModernApplicationFramework.EditorBase.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(SaveAllCommandDefinition))]
    public class SaveAllCommandDefinition : CommandDefinition
    {
        private readonly IDockingHostViewModel _dockingHostViewModel;

        public override string NameUnlocalized => "Save active file";

        public override string Text => CommandsResources.SaveAllCommandText;

        public override string ToolTip => Text;

        public override Uri IconSource => new Uri("/ModernApplicationFramework.EditorBase;component/Resources/Icons/SaveAll_16x.xaml",
            UriKind.RelativeOrAbsolute);
        public override string IconId => "SaveAllFilesIcon";
        public override CommandCategory Category => CommandCategories.FileCommandCategory;
        public override Guid Id => new Guid("{651EA782-BFCB-4ACA-8F98-6798C117F988}");
        public override MultiKeyGesture DefaultKeyGesture { get; }
        public override GestureScope DefaultGestureScope { get; }

        public override UICommand Command { get; }

        [ImportingConstructor]
        public SaveAllCommandDefinition(IDockingHostViewModel dockingHostViewModel)
        {
            _dockingHostViewModel = dockingHostViewModel;
            var command = new UICommand(SaveAll, CanSaveAll);
            Command = command;
            DefaultKeyGesture = new MultiKeyGesture(Key.S, ModifierKeys.Control | ModifierKeys.Shift);
            DefaultGestureScope = GestureScopes.GlobalGestureScope;
        }


        private bool CanSaveAll()
        {
            if (_dockingHostViewModel.LayoutItems == null || _dockingHostViewModel.LayoutItems.Count == 0)
                return false;
            return _dockingHostViewModel.LayoutItems.Any(x => x is IEditor editor && editor.Document is IStorableFile);
        }

        private void SaveAll()
        {
            foreach (var editor in _dockingHostViewModel.LayoutItems.OfType<IEditor>().Where(x => x.Document is IStorableFile))
            {
                editor.SaveFile();
            }         
        }
    }
}
