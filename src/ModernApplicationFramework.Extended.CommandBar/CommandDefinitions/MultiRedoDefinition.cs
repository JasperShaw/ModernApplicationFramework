using System;
using System.ComponentModel.Composition;
using System.Globalization;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Creators;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.CommandBar.Models;
using ModernApplicationFramework.Basics.Definitions.ItemDefinitions;
using ModernApplicationFramework.Extended.CommandBar.Resources;
using ModernApplicationFramework.Extended.Commands;
using ModernApplicationFramework.Extended.UndoRedoManager;
using ModernApplicationFramework.ImageCatalog;
using ModernApplicationFramework.Imaging.Interop;

namespace ModernApplicationFramework.Extended.CommandBar.CommandDefinitions
{
    [Export(typeof(CommandBarItemDefinition))]
    [Export(typeof(MultiRedoDefinition))]
    public sealed class MultiRedoDefinition : SplitButtonDefinition<IMultiRedoCommand>
    {
        public override ImageMoniker ImageMonikerSource => Monikers.Redo;

        public override string Name => Commands_Resources.MultiRedoCommandDefinition_Name;
        public override string Text => Commands_Resources.MultiRedoCommandDefinition_Text;

        public override string NameUnlocalized =>
            Commands_Resources.ResourceManager.GetString("MultiRedoCommandDefinition_Text",
                CultureInfo.InvariantCulture);
        public override string ToolTip => Commands_Resources.MultiRedoCommandDefinition_ToolTip;

        public override CommandBarCategory Category => CommandCategories.EditCategory;
        public override Guid Id => new Guid("{7225AF5F-4039-4686-89EC-71AFE605FEF5}");

        public override bool AllowGestureMapping => false;

        [ImportingConstructor]
        public MultiRedoDefinition(CommandBarUndoRedoManagerWatcher watcher)
        {
            //Items = watcher.RedoItems;

            var statusCrator = new NumberStatusStringCreator(Commands_Resources.MultiRedoCommandDefinition_StatusText,
                Commands_Resources.MultiRedoCommandDefinition_StatusSuffix);

            Model = new SplitButtonModel(watcher.RedoItems, statusCrator);
        }

        public override SplitButtonModel Model { get; }
    }
}