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
    internal class WordNextExtendCommandDefinition : CommandDefinition
    {
        [ImportingConstructor]
        public WordNextExtendCommandDefinition() : base(new TextEditCommand(MafConstants.EditorCommands.WordNextExtend))
        {
        }

        public override string NameUnlocalized => "WordNextExtend";
        public override string Text => NameUnlocalized;
        public override string ToolTip => Text;
        public override CommandBarCategory Category => CommandBarCategories.EditCategory;
        public override Guid Id => new Guid("{D18F3527-1468-4146-813E-031EFB453735}");

        public override ReadOnlyCollection<GestureScopeMapping> DefaultGestureScopes => new ReadOnlyCollection<GestureScopeMapping>(new[]
        {
            new GestureScopeMapping(TextEditorGestureScope.TextEditorScope, new MultiKeyGesture(Key.Right, ModifierKeys.Control | ModifierKeys.Shift))
        });
    }
}