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
    internal class ToggleOverTypeModeCommandDefinition : CommandDefinition
    {
        [ImportingConstructor]
        public ToggleOverTypeModeCommandDefinition() : base(new TextEditCommand(MafConstants.EditorCommands.ToggleOverTypeMode))
        {
        }

        public override string NameUnlocalized => "Overtype mode";
        public override string Text => NameUnlocalized;
        public override string ToolTip => Text;
        public override CommandBarCategory Category => CommandBarCategories.EditCategory;
        public override Guid Id => new Guid("{C27D02A8-F7E5-45CD-A325-D64EA98C5525}");

        public override bool Checkable => true;

        public override ReadOnlyCollection<GestureScopeMapping> DefaultGestureScopes => new ReadOnlyCollection<GestureScopeMapping>(new[]
        {
            new GestureScopeMapping(TextEditorGestureScope.TextEditorScope, new MultiKeyGesture(Key.Insert))
        });
    }
}
