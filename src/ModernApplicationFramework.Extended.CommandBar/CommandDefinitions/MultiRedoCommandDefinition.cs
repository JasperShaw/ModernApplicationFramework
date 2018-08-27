﻿using System;
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
using ModernApplicationFramework.ImageCatalog;
using ModernApplicationFramework.Imaging.Interop;

namespace ModernApplicationFramework.Extended.CommandBar.CommandDefinitions
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(MultiRedoCommandDefinition))]
    public sealed class MultiRedoCommandDefinition : CommandSplitButtonDefinition<IMultiRedoCommand>
    {
        public override ImageMoniker ImageMonikerSource => Monikers.Redo;

        public override string Name => Commands_Resources.MultiRedoCommandDefinition_Name;
        public override string Text => Commands_Resources.MultiRedoCommandDefinition_Text;

        public override string NameUnlocalized =>
            Commands_Resources.ResourceManager.GetString("MultiRedoCommandDefinition_Text",
                CultureInfo.InvariantCulture);
        public override string ToolTip => Commands_Resources.MultiRedoCommandDefinition_ToolTip;

        public override CommandCategory Category => CommandCategories.EditCommandCategory;
        public override Guid Id => new Guid("{7225AF5F-4039-4686-89EC-71AFE605FEF5}");

        public override bool AllowGestureMapping => false;

        [ImportingConstructor]
        public MultiRedoCommandDefinition(CommandBarUndoRedoManagerWatcher watcher)
        {
            //Items = watcher.RedoItems;

            var statusCrator = new NumberStatusStringCreator(Commands_Resources.MultiRedoCommandDefinition_StatusText,
                Commands_Resources.MultiRedoCommandDefinition_StatusSuffix);

            Model = new SplitButtonModel(watcher.RedoItems, statusCrator);
        }

        public override SplitButtonModel Model { get; }
    }
}