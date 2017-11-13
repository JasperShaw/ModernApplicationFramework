using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.WindowManagement.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(ApplyWindowLayout4))]
    public sealed class ApplyWindowLayout4 : ApplyWindowLayoutBase
    {
        public override int Index => 4;
        public override MultiKeyGesture DefaultKeyGesture { get; }
        public override GestureScope DefaultGestureScope { get; }

        [ImportingConstructor]
        public ApplyWindowLayout4()
        {
            DefaultKeyGesture = new MultiKeyGesture(Key.D4, ModifierKeys.Control | ModifierKeys.Alt);
            DefaultGestureScope = GestureScopes.GlobalGestureScope;
        }

        public override Guid Id => new Guid("{B38DCA4B-734B-43BA-80D8-1B691810742E}");
    }
}