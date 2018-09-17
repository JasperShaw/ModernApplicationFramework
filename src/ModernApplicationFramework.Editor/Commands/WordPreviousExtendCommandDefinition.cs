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
    internal class WordPreviousExtendCommandDefinition : CommandDefinition
    {
        [ImportingConstructor]
        public WordPreviousExtendCommandDefinition() : base(new TextEditCommand(MafConstants.EditorCommands.WordPreviousExtend))
        {
        }

        public override string NameUnlocalized => "WordPreviousExtend";
        public override string Text => NameUnlocalized;
        public override string ToolTip => Text;
        public override CommandBarCategory Category => CommandBarCategories.EditCategory;
        public override Guid Id => new Guid("{44656E18-45E7-4B72-85B8-107686017CF6}");

        public override ReadOnlyCollection<GestureScopeMapping> DefaultGestureScopes => new ReadOnlyCollection<GestureScopeMapping>(new[]
        {
            new GestureScopeMapping(TextEditorGestureScope.TextEditorScope, new MultiKeyGesture(Key.Left, ModifierKeys.Control | ModifierKeys.Shift))
        });
    }
}