using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Extended.CommandBar.Resources;
using ModernApplicationFramework.Extended.Commands;
using ModernApplicationFramework.ImageCatalog;
using ModernApplicationFramework.Imaging.Interop;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Extended.CommandBar.CommandDefinitions
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(FullScreenCommandDefinition))]
    public sealed class FullScreenCommandDefinition : CommandDefinition<IFullScreenCommand>
    {
        public override ReadOnlyCollection<GestureScopeMapping> DefaultGestureScopes => new ReadOnlyCollection<GestureScopeMapping>(new[]
        {
            new GestureScopeMapping(GestureScopes.GlobalGestureScope, new MultiKeyGesture(Key.Enter, ModifierKeys.Shift | ModifierKeys.Alt))
        });

        public override ImageMoniker ImageMonikerSource => Monikers.FitToScreen;

        public override string NameUnlocalized =>
            Commands_Resources.ResourceManager.GetString("FullScreenCommandDefinition_Text",
                CultureInfo.InvariantCulture);

        public override string Text => Commands_Resources.FullScreenCommandDefinition_Text;
        public override string ToolTip => Text;

        public override CommandCategory Category => CommandCategories.ViewCommandCategory;
        public override Guid Id => new Guid("{9EE995EC-45C6-40B9-A3D6-8A9F486D59C9}");
    }
}