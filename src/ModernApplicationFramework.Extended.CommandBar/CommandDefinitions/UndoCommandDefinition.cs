using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Extended.CommandBar.Resources;
using ModernApplicationFramework.Extended.Commands;
using ModernApplicationFramework.Imaging;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Extended.CommandBar.CommandDefinitions
{
    [Export(typeof(CommandBarItemDefinition))]
    [Export(typeof(UndoCommandDefinition))]
    public sealed class UndoCommandDefinition : CommandDefinition<IUndoCommand>
    {
        public override ReadOnlyCollection<GestureScopeMapping> DefaultGestureScopes => new ReadOnlyCollection<GestureScopeMapping>(new[]
        {
            new GestureScopeMapping(GestureScopes.GlobalGestureScope, new MultiKeyGesture(Key.Z, ModifierKeys.Control))
        });

        public override Imaging.Interop.ImageMoniker ImageMonikerSource => ImageCatalog.Monikers.Undo;

        public override string Text => Commands_Resources.UndoCommandDefinition_Text;
        public override string NameUnlocalized =>
            Commands_Resources.ResourceManager.GetString("UndoCommandDefinition_Text",
                CultureInfo.InvariantCulture);
        public override string ToolTip => Text;

        public override CommandBarCategory Category => CommandCategories.EditCategory;
        public override Guid Id => new Guid("{1A236C59-DA8D-424F-804B-22D80CFA15D6}");
    }
}