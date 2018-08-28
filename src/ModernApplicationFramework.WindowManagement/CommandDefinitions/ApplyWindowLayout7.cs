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
    [Export(typeof(ApplyWindowLayout7))]
    public sealed class ApplyWindowLayout7 : ApplyWindowLayoutBase
    {
        public override int Index => 7;

        public override ReadOnlyCollection<GestureScopeMapping> DefaultGestureScopes => new ReadOnlyCollection<GestureScopeMapping>(new[]
        {
            new GestureScopeMapping(GestureScopes.GlobalGestureScope, new MultiKeyGesture(Key.D7, ModifierKeys.Control | ModifierKeys.Alt))
        });

        [ImportingConstructor]
        public ApplyWindowLayout7()
        {
            SetCommand();
        }

        public override Guid Id => new Guid("{3F678A92-83CF-4152-A42E-B7EC32B9C453}");
    }
}