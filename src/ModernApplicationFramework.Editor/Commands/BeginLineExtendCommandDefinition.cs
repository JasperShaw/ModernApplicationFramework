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
    internal class BeginLineExtendCommandDefinition : CommandDefinition
    {
        [ImportingConstructor]
        public BeginLineExtendCommandDefinition() : base(new TextEditCommand(MafConstants.EditorCommands.BeginOfLineExt))
        {
        }

        public override string NameUnlocalized => "Begin Line Extension";
        public override string Text => NameUnlocalized;
        public override string ToolTip => Text;
        public override CommandBarCategory Category => CommandBarCategories.EditCategory;
        public override Guid Id => new Guid("{F3E99EA3-07AB-4639-8C9F-15F78B52845C}");

        public override ReadOnlyCollection<GestureScopeMapping> DefaultGestureScopes => new ReadOnlyCollection<GestureScopeMapping>(new[]
        {
            new GestureScopeMapping(TextEditorGestureScope.TextEditorScope, new MultiKeyGesture(Key.Home, ModifierKeys.Shift))
        });
    }
}