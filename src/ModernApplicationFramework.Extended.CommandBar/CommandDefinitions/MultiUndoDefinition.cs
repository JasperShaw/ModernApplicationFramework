using System;
using System.ComponentModel.Composition;
using System.Globalization;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
using ModernApplicationFramework.Basics.CommandBar.Models;
using ModernApplicationFramework.Basics.Creators;
using ModernApplicationFramework.Extended.CommandBar.Resources;
using ModernApplicationFramework.Extended.Commands;
using ModernApplicationFramework.Extended.UndoRedoManager;

namespace ModernApplicationFramework.Extended.CommandBar.CommandDefinitions
{
    [Export(typeof(CommandBarItemDefinition))]
    [Export(typeof(MultiUndoDefinition))]
    public sealed class MultiUndoDefinition : SplitButtonDefinition<IMultiUndoCommand>
    {
        public override Imaging.Interop.ImageMoniker ImageMonikerSource => ImageCatalog.Monikers.Undo;

        public override string Name => Commands_Resources.MultiUndoCommandDefinition_Name;
        public override string Text => Commands_Resources.MultiUndoCommandDefinition_Text;

        public override string NameUnlocalized =>
            Commands_Resources.ResourceManager.GetString("MultiUndoCommandDefinition_Text",
                CultureInfo.InvariantCulture);
        public override string ToolTip => Commands_Resources.MultiUndoCommandDefinition_ToolTip;

        public override CommandBarCategory Category => CommandCategories.EditCategory;
        public override Guid Id => new Guid("{D2043E14-F0AF-4C12-933A-F753BA1F9488}");

        public override bool AllowGestureMapping => false;

        [ImportingConstructor]
        public MultiUndoDefinition(CommandBarUndoRedoManagerWatcher watcher)
        {
            var statusCreator = new NumberStatusStringCreator(Commands_Resources.MultiUndoCommandDefinition_StatusText,
                Commands_Resources.MultiUndoCommandDefinition_StatusSuffix);

            Model = new SplitButtonModel(watcher.UndoItems,statusCreator);
        }

        public override SplitButtonModel Model { get; }
    }
}