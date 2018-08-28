using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics.Definitions.ItemDefinitions;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.WindowManagement.CommandDefinitions
{
    [Export(typeof(CommandBarItemDefinition))]
    [Export(typeof(ApplyWindowLayout8))]
    public sealed class ApplyWindowLayout8 : ApplyWindowLayoutBase
    {
        public override int Index => 8;

        public override ReadOnlyCollection<GestureScopeMapping> DefaultGestureScopes => new ReadOnlyCollection<GestureScopeMapping>(new[]
        {
            new GestureScopeMapping(GestureScopes.GlobalGestureScope, new MultiKeyGesture(Key.D8, ModifierKeys.Control | ModifierKeys.Alt))
        });

        [ImportingConstructor]
        public ApplyWindowLayout8()
        {
            SetCommand();
        }

        public override Guid Id => new Guid("{E95C96CE-2D44-4EE7-AC18-0AFBA0BFA3F7}");
    }
}