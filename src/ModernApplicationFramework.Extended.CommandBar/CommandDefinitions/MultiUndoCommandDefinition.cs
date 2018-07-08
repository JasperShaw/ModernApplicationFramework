using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using Caliburn.Micro;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Creators;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Extended.CommandBar.Resources;
using ModernApplicationFramework.Extended.Commands;
using ModernApplicationFramework.Extended.UndoRedoManager;
using ModernApplicationFramework.Imaging;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.Utilities;

namespace ModernApplicationFramework.Extended.CommandBar.CommandDefinitions
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(MultiUndoCommandDefinition))]
    public sealed class MultiUndoCommandDefinition : CommandSplitButtonDefinition<IMultiUndoCommand>
    {
        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures => null;
        public override GestureScope DefaultGestureScope => null;

        public override string IconId => "UndoIcon";

        public override Uri IconSource
            =>
                new Uri("/ModernApplicationFramework.Extended.CommandBar;component/Resources/Icons/Undo_16x.xaml",
                    UriKind.RelativeOrAbsolute);


        public override Imaging.Interop.ImageMoniker ImageMonikerSource => ImageCatalog.Monikers.Undo;


        public override string Name => Commands_Resources.MultiUndoCommandDefinition_Name;
        public override string Text => Commands_Resources.MultiUndoCommandDefinition_Text;

        public override string NameUnlocalized =>
            Commands_Resources.ResourceManager.GetString("MultiUndoCommandDefinition_Text",
                CultureInfo.InvariantCulture);
        public override string ToolTip => Commands_Resources.MultiUndoCommandDefinition_ToolTip;

        public override CommandCategory Category => CommandCategories.EditCommandCategory;
        public override Guid Id => new Guid("{D2043E14-F0AF-4C12-933A-F753BA1F9488}");

        [ImportingConstructor]
        public MultiUndoCommandDefinition(CommandBarUndoRedoManagerWatcher watcher)
        {
            Items = watcher.UndoItems;
        }

        public override IObservableCollection<IHasTextProperty> Items { get; set; }

        public override IStatusStringCreator StatusStringCreator =>
            new NumberStatusStringCreator(Commands_Resources.MultiUndoCommandDefinition_StatusText,
                Commands_Resources.MultiRedoCommandDefinition_StatusSuffix);
    }
}