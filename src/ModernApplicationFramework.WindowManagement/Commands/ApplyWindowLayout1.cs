using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.WindowManagement.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(ApplyWindowLayout1))]
    public sealed class ApplyWindowLayout1 : ApplyWindowLayoutBase
    {
        public override int Index => 1;
        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures { get; }
        public override GestureScope DefaultGestureScope { get; }


        [ImportingConstructor]
        public ApplyWindowLayout1()
        {
            DefaultKeyGestures = new []{ new MultiKeyGesture(Key.D1, ModifierKeys.Control | ModifierKeys.Alt)};
            DefaultGestureScope = GestureScopes.GlobalGestureScope;
        }

        public override Guid Id => new Guid("{C17FB832-108A-4863-9CD0-F3596BAC1CF3}");
    }
}