using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics.CommandBar;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
using ModernApplicationFramework.Editor.Commanding;
using ModernApplicationFramework.Input;

namespace ModernApplicationFramework.Editor.Commands
{
    [Export(typeof(CommandBarItemDefinition))]
    internal class SwapAnchorCommandDefinition : CommandDefinition
    {
        [ImportingConstructor]
        public SwapAnchorCommandDefinition() : base(new TextEditCommand(MafConstants.EditorCommands.SwapAnchor))
        {
        }

        public override string NameUnlocalized => "Swap Anchor";
        public override string Text => NameUnlocalized;
        public override string ToolTip => Text;
        public override CommandBarCategory Category => CommandBarCategories.EditCategory;
        public override Guid Id => new Guid("{A40E21EC-E4BE-4749-999F-2BF6B8DD1554}");

        public override ReadOnlyCollection<GestureScopeMapping> DefaultGestureScopes => new ReadOnlyCollection<GestureScopeMapping>(new[]
        {
            new GestureScopeMapping(TextEditorGestureScope.TextEditorScope, new MultiKeyGesture(new List<KeySequence>
            {
                new KeySequence(ModifierKeys.Control, Key.K),
                new KeySequence(ModifierKeys.Control, Key.A)
            }))
        });
    }
}