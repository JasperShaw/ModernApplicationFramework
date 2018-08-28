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
    [Export(typeof(ApplyWindowLayout9))]
    public sealed class ApplyWindowLayout9 : ApplyWindowLayoutBase
    {
        public override int Index => 9;

        public override ReadOnlyCollection<GestureScopeMapping> DefaultGestureScopes => new ReadOnlyCollection<GestureScopeMapping>(new[]
        {
            new GestureScopeMapping(GestureScopes.GlobalGestureScope, new MultiKeyGesture(Key.D9, ModifierKeys.Control | ModifierKeys.Alt))
        });

        [ImportingConstructor]
        public ApplyWindowLayout9()
        {
            SetCommand();
        }

        public override Guid Id => new Guid("{0A351097-2140-46B8-9A24-7CE7321C9B92}");
    }
}