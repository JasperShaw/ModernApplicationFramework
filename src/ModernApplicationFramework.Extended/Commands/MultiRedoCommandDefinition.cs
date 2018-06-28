using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using Caliburn.Micro;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Creators;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Extended.Properties;
using ModernApplicationFramework.Extended.UndoRedoManager;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.Utilities;

namespace ModernApplicationFramework.Extended.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(MultiRedoCommandDefinition))]
    public sealed class MultiRedoCommandDefinition : CommandSplitButtonDefinition<IMultiRedoCommand>
    {
        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures => null;
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
            Items = watcher.RedoItems;
        }

        public override IObservableCollection<IHasTextProperty> Items { get; set; }

        public override IStatusStringCreator StatusStringCreator =>
            new NumberStatusStringCreator(Commands_Resources.MultiRedoCommandDefinition_StatusText,
                Commands_Resources.MultiRedoCommandDefinition_StatusSuffix);

    }

    public interface IMultiRedoCommand : ICommandDefinitionCommand
    {
    }

    [Export(typeof(IMultiRedoCommand))]
    internal class MultiRedoCommand : CommandDefinitionCommand, IMultiRedoCommand
    {
        private readonly CommandBarUndoRedoManagerWatcher _watcher;

        [ImportingConstructor]
        public MultiRedoCommand(CommandBarUndoRedoManagerWatcher watcher)
        {
            _watcher = watcher;
        }

        protected override bool OnCanExecute(object parameter)
        {
            return _watcher.UndoRedoManager != null && _watcher.UndoRedoManager.RedoStack.Any();
        }

        protected override void OnExecute(object parameter)
        {
            if (!(parameter is int number))
                _watcher.UndoRedoManager.Redo(1);
            else
                _watcher.UndoRedoManager.Redo(number);
        }
    }
}