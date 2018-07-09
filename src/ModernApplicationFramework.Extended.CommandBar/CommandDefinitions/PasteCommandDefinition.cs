using System;
using System.Collections.Generic;
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
    public class PasteCommandDefinition : CommandDefinition<IPasteCommand>
    {
        public override string NameUnlocalized => Commands_Resources.ResourceManager.GetString(nameof(Commands_Resources.PasteCommandDefinition_Text),
            CultureInfo.InvariantCulture);
        public override string Text => Commands_Resources.PasteCommandDefinition_Text;
        public override string ToolTip => Text;
        public override ImageMoniker ImageMonikerSource => Monikers.Paste;
        public override CommandCategory Category => CommandCategories.EditCommandCategory;
        public override Guid Id => new Guid("{7820A125-F085-4338-81B3-22985BE24B55}");

        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures =>
            new[]
            {
                new MultiKeyGesture(Key.V, ModifierKeys.Control),
                new MultiKeyGesture(Key.Insert, ModifierKeys.Shift)
            };
        public override GestureScope DefaultGestureScope => GestureScopes.GlobalGestureScope;
    }
}
