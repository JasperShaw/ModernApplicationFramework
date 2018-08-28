using System;
using System.ComponentModel.Composition;
using System.Globalization;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Creators;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.CommandBar.Models;
using ModernApplicationFramework.Extended.CommandBar.Resources;
using ModernApplicationFramework.Extended.Commands;
using ModernApplicationFramework.Extended.UndoRedoManager;

namespace ModernApplicationFramework.Extended.CommandBar.CommandDefinitions
{
    [Export(typeof(CommandBarItemDefinition))]
    [Export(typeof(MultiUndoCommandDefinition))]
    public sealed class MultiUndoCommandDefinition : CommandSplitButtonDefinition<IMultiUndoCommand>
    {
        public override Imaging.Interop.ImageMoniker ImageMonikerSource => ImageCatalog.Monikers.Undo;

        public override string Name => Commands_Resources.MultiUndoCommandDefinition_Name;
        public override string Text => Commands_Resources.MultiUndoCommandDefinition_Text;

        public override string NameUnlocalized =>
            Commands_Resources.ResourceManager.GetString("MultiUndoCommandDefinition_Text",
                CultureInfo.InvariantCulture);
        public override string ToolTip => Commands_Resources.MultiUndoCommandDefinition_ToolTip;

        public override CommandCategory Category => CommandCategories.EditCommandCategory;
        public override Guid Id => new Guid("{D2043E14-F0AF-4C12-933A-F753BA1F9488}");

        public override bool AllowGestureMapping => false;

        [ImportingConstructor]
        public MultiUndoCommandDefinition(CommandBarUndoRedoManagerWatcher watcher)
        {
            var statusCreator = new NumberStatusStringCreator(Commands_Resources.MultiUndoCommandDefinition_StatusText,
                Commands_Resources.MultiUndoCommandDefinition_StatusSuffix);

            Model = new SplitButtonModel(watcher.UndoItems,statusCreator);
        }

        public override SplitButtonModel Model { get; }
    }
}