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
    [Export(typeof(ApplyWindowLayout10))]
    public sealed class ApplyWindowLayout10 : ApplyWindowLayoutBase
    {
        public override int Index => 10;
        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures { get; }
        public override GestureScope DefaultGestureScope { get; }

        [ImportingConstructor]
        public ApplyWindowLayout10()
        {
            DefaultKeyGestures = new []{new MultiKeyGesture(Key.D0, ModifierKeys.Control | ModifierKeys.Alt)};
            DefaultGestureScope = GestureScopes.GlobalGestureScope;
        }

        public override Guid Id => new Guid("{0E6A3346-1E9C-401B-9443-C63281B6B298}");
    }
}