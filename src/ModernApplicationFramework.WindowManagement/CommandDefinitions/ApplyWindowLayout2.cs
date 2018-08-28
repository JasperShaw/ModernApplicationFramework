using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.WindowManagement.CommandDefinitions
{
    [Export(typeof(CommandBarItemDefinition))]
    [Export(typeof(ApplyWindowLayout2))]
    public sealed class ApplyWindowLayout2 : ApplyWindowLayoutBase
    {
        public override int Index => 2;

        public override ReadOnlyCollection<GestureScopeMapping> DefaultGestureScopes => new ReadOnlyCollection<GestureScopeMapping>(new[]
        {
            new GestureScopeMapping(GestureScopes.GlobalGestureScope, new MultiKeyGesture(Key.D2, ModifierKeys.Control | ModifierKeys.Alt))
        });


        [ImportingConstructor]
        public ApplyWindowLayout2()
        {
            SetCommand();
        }

        public override Guid Id => new Guid("{5E8E46EB-06BF-498A-9908-BC4C577A4790}");
    }
}