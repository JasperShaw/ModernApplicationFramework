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
    [Export(typeof(ApplyWindowLayout2))]
    public sealed class ApplyWindowLayout2 : ApplyWindowLayoutBase
    {
        public override int Index => 2;
        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures { get; }
        public override GestureScope DefaultGestureScope { get; }


        [ImportingConstructor]
        public ApplyWindowLayout2()
        {
            DefaultKeyGestures = new []{ new MultiKeyGesture(Key.D2, ModifierKeys.Control | ModifierKeys.Alt)};
            DefaultGestureScope = GestureScopes.GlobalGestureScope;
        }

        public override Guid Id => new Guid("{5E8E46EB-06BF-498A-9908-BC4C577A4790}");
    }
}