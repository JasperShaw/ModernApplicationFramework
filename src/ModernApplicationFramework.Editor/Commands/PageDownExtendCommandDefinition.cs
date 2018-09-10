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
    internal class PageDownExtendCommandDefinition : CommandDefinition
    {
        [ImportingConstructor]
        public PageDownExtendCommandDefinition() : base(new TextEditCommand(MafConstants.EditorCommands.PageDownExt))
        {
        }

        public override string NameUnlocalized => "Page Down Extend";
        public override string Text => NameUnlocalized;
        public override string ToolTip => Text;
        public override CommandBarCategory Category => CommandBarCategories.EditCategory;
        public override Guid Id => new Guid("{0E823118-82C1-4C56-A493-8409ADC2F7C4}");

        public override ReadOnlyCollection<GestureScopeMapping> DefaultGestureScopes => new ReadOnlyCollection<GestureScopeMapping>(new[]
        {
            new GestureScopeMapping(TextEditorGestureScope.TextEditorScope, new MultiKeyGesture(Key.PageDown, ModifierKeys.Shift))
        });
    }
}