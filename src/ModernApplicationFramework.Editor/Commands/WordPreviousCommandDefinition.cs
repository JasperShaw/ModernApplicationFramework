using System;
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
    internal class WordPreviousCommandDefinition : CommandDefinition
    {
        [ImportingConstructor]
        public WordPreviousCommandDefinition() : base(new TextEditCommand(MafConstants.EditorCommands.WordPrevious))
        {
        }

        public override string NameUnlocalized => "WordPrevious";
        public override string Text => NameUnlocalized;
        public override string ToolTip => Text;
        public override CommandBarCategory Category => CommandBarCategories.EditCategory;
        public override Guid Id => new Guid("{04D9B768-AA92-442A-A717-8521D5C1241B}");

        public override ReadOnlyCollection<GestureScopeMapping> DefaultGestureScopes => new ReadOnlyCollection<GestureScopeMapping>(new[]
        {
            new GestureScopeMapping(TextEditorGestureScope.TextEditorScope, new MultiKeyGesture(Key.Left, ModifierKeys.Control))
        });
    }
}