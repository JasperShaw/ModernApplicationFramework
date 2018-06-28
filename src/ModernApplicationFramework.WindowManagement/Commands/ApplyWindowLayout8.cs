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
    [Export(typeof(ApplyWindowLayout8))]
    public sealed class ApplyWindowLayout8 : ApplyWindowLayoutBase
    {
        public override int Index => 8;
        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures { get; }
        public override GestureScope DefaultGestureScope { get; }

        [ImportingConstructor]
        public ApplyWindowLayout8()
        {
            DefaultKeyGestures = new []{ new MultiKeyGesture(Key.D8, ModifierKeys.Control | ModifierKeys.Alt)};
            DefaultGestureScope = GestureScopes.GlobalGestureScope;
        }

        public override Guid Id => new Guid("{E95C96CE-2D44-4EE7-AC18-0AFBA0BFA3F7}");
    }
}