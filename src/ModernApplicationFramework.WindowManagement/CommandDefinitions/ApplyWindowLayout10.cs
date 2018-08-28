using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.WindowManagement.CommandDefinitions
{
    [Export(typeof(CommandBarItemDefinition))]
    [Export(typeof(ApplyWindowLayout10))]
    public sealed class ApplyWindowLayout10 : ApplyWindowLayoutBase
    {
        public override int Index => 10;

        public override ReadOnlyCollection<GestureScopeMapping> DefaultGestureScopes => new ReadOnlyCollection<GestureScopeMapping>(new[]
        {
            new GestureScopeMapping(GestureScopes.GlobalGestureScope, new MultiKeyGesture(Key.D0, ModifierKeys.Control | ModifierKeys.Alt))
        });

        [ImportingConstructor]
        public ApplyWindowLayout10()
        {
            SetCommand();
        }

        public override Guid Id => new Guid("{0E6A3346-1E9C-401B-9443-C63281B6B298}");
    }
}