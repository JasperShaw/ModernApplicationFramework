using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Editor.Commands
{
    //[Export(typeof(CommandDefinitionBase))]
    internal class LeftCommandDefinition : CommandDefinition
    {
        public override string NameUnlocalized => "LeftMove";
        public override string Text => NameUnlocalized;
        public override string ToolTip => Text;
        public override CommandCategory Category => CommandCategories.EditCommandCategory;
        public override Guid Id => new Guid("{3CBBD74E-9DA9-4D4D-A0BB-77DEB8A6636B}");
        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures => new[] {new MultiKeyGesture(Key.Left)};
        public override GestureScope DefaultGestureScope => GestureScopes.GlobalGestureScope;
    }
}
