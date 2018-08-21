using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.ImageCatalog;
using ModernApplicationFramework.Imaging.Interop;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Modules.Output
{
    [Export(typeof(CommandDefinitionBase))]
    public sealed class OpenOutputToolCommandDefinition : CommandDefinition<IOpenOutputToolCommand>
    {
        public override ReadOnlyCollection<GestureScopeMapping> DefaultGestureScopes => new ReadOnlyCollection<GestureScopeMapping>(new[]
        {
            new GestureScopeMapping(GestureScopes.GlobalGestureScope, new MultiKeyGesture(Key.O, ModifierKeys.Control | ModifierKeys.Alt))
        });

        public override ImageMoniker ImageMonikerSource => Monikers.Output;

        public override string Name => "View.Output";
        public override string NameUnlocalized => "Output";
        public override string Text => "Output";
        public override string ToolTip => "Output";

        public override CommandCategory Category => CommandCategories.ViewCommandCategory;
        public override Guid Id => new Guid("{ED3DC8E1-F15B-4DBD-8C8E-272194C0642D}");
    }
}