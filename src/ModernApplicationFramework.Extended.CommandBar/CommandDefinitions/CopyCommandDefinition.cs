using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Extended.CommandBar.Resources;
using ModernApplicationFramework.Extended.Interfaces.Commands;
using ModernApplicationFramework.ImageCatalog;
using ModernApplicationFramework.Imaging.Interop;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Extended.CommandBar.CommandDefinitions
{
    [Export(typeof(CommandDefinitionBase))]
    public class CopyCommandDefinition : CommandDefinition<ICopyCommand>
    {
        public override string NameUnlocalized => Commands_Resources.ResourceManager.GetString(nameof(Commands_Resources.CopyCommandDefinition_Text),
            CultureInfo.InvariantCulture);

        public override string Text => Commands_Resources.CopyCommandDefinition_Text;
        public override string ToolTip => Text;
        public override ImageMoniker ImageMonikerSource => Monikers.Copy;
        public override CommandCategory Category => CommandCategories.EditCommandCategory;
        public override Guid Id => new Guid("{E98D986F-AACB-4BC7-A60B-E758CA847BA9}");

        public override ReadOnlyCollection<GestureScopeMapping> DefaultGestureScopes => new ReadOnlyCollection<GestureScopeMapping>(new[]
        {
            new GestureScopeMapping(GestureScopes.GlobalGestureScope, new MultiKeyGesture(Key.C, ModifierKeys.Control)),
            new GestureScopeMapping(GestureScopes.GlobalGestureScope, new MultiKeyGesture(Key.Insert, ModifierKeys.Control))
        });
    }
}
