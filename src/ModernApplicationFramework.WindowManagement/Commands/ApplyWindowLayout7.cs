using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.WindowManagement.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(ApplyWindowLayout7))]
    public sealed class ApplyWindowLayout7 : ApplyWindowLayoutBase
    {
        public override int Index => 7;
        public override MultiKeyGesture DefaultKeyGesture { get; }
        public override GestureScope DefaultGestureScope { get; }

        [ImportingConstructor]
        public ApplyWindowLayout7()
        {
            DefaultKeyGesture = new MultiKeyGesture(Key.D7, ModifierKeys.Control | ModifierKeys.Alt);
            DefaultGestureScope = GestureScopes.GlobalGestureScope;
        }

        public override Guid Id => new Guid("{3F678A92-83CF-4152-A42E-B7EC32B9C453}");
    }
}