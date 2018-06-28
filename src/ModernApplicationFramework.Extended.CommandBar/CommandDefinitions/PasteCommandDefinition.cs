using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Extended.Commands;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Extended.CommandBar.CommandDefinitions
{
    [Export(typeof(CommandDefinitionBase))]
    public class PasteCommandDefinition : CommandDefinition<IPasteCommand>
    {
        public override string NameUnlocalized => "Paste";
        public override string Text => "Paste";
        public override string ToolTip => Text;
        public override Uri IconSource => new Uri("/ModernApplicationFramework.Extended.CommandBar;component/Resources/Icons/Paste_16x.xaml",
            UriKind.RelativeOrAbsolute);
        public override string IconId => "PasteIcon";
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
