using System;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using Caliburn.Micro;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Extended.Core;
using ModernApplicationFramework.Extended.Properties;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Extended.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(MultiRedoCommandDefinition))]
    public sealed class MultiRedoCommandDefinition : CommandSplitButtonDefinition
    {
        private readonly CommandBarUndoRedoManagerWatcher _watcher;

        public override UICommand Command { get; }

        public override MultiKeyGesture DefaultKeyGesture => null;
        public override GestureScope DefaultGestureScope => null;

        public override string IconId => "RedoIcon";

        public override Uri IconSource
            =>
                new Uri("/ModernApplicationFramework.Extended;component/Resources/Icons/Redo_16x.xaml",
                    UriKind.RelativeOrAbsolute);

        public override string Name => Commands_Resources.MultiRedoCommandDefinition_Name;
        public override string Text => Commands_Resources.MultiRedoCommandDefinition_Text;

        public override string NameUnlocalized =>
            Commands_Resources.ResourceManager.GetString("MultiRedoCommandDefinition_Text",
                CultureInfo.InvariantCulture);
        public override string ToolTip => Commands_Resources.MultiRedoCommandDefinition_ToolTip;

        public override CommandCategory Category => CommandCategories.EditCommandCategory;
        public override Guid Id => new Guid("{7225AF5F-4039-4686-89EC-71AFE605FEF5}");

        [ImportingConstructor]
        public MultiRedoCommandDefinition(CommandBarUndoRedoManagerWatcher watcher)
        {
            var command = new UICommand(Redo, CanRedo);
            Command = command;

            _watcher = watcher;
            Items = _watcher.RedoItems;
        }

        private bool CanRedo()
        {
            return _watcher.UndoRedoManager != null && _watcher.UndoRedoManager.RedoStack.Any();
        }

        private void Redo()
        {
            _watcher.UndoRedoManager.Redo(1);
        }

        public override IObservableCollection<IHasTextProperty> Items { get; set; }
        public override void Execute(int count)
        {
            _watcher.UndoRedoManager?.Redo(count);
        }
    }
}