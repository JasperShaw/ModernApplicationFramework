using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace MordernApplicationFramework.WindowManagement.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    public sealed class ApplyWindowLayout9 : ApplyWindowLayoutBase
    {
        public override int Index => 9;
        public override MultiKeyGesture DefaultKeyGesture { get; }
        public override GestureScope DefaultGestureScope { get; }

        public override string Text { get; }

        protected override ILayoutManager LayoutManager { get; }

        [ImportingConstructor]
        public ApplyWindowLayout9(ILayoutManager layoutManager)
        {
            LayoutManager = layoutManager;
            DefaultKeyGesture = new MultiKeyGesture(Key.D9, ModifierKeys.Control | ModifierKeys.Alt);
            DefaultGestureScope = GestureScopes.GlobalGestureScope;
        }

        public ApplyWindowLayout9(string text, ILayoutManager layoutManager) : this(layoutManager)
        {
            Text = text;
        }
    }
}