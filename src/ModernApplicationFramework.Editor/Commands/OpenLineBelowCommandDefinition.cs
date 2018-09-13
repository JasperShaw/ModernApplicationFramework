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
    internal class OpenLineBelowCommandDefinition : CommandDefinition
    {
        [ImportingConstructor]
        public OpenLineBelowCommandDefinition() : base(new TextEditCommand(MafConstants.EditorCommands.OpenLineBelow))
        {
        }

        public override string NameUnlocalized => "OpenLineBelow";
        public override string Text => NameUnlocalized;
        public override string ToolTip => Text;
        public override CommandBarCategory Category => CommandBarCategories.EditCategory;
        public override Guid Id => new Guid("{67AFDB95-7640-487A-A557-C25B22694BEC}");

        public override ReadOnlyCollection<GestureScopeMapping> DefaultGestureScopes => new ReadOnlyCollection<GestureScopeMapping>(new[]
        {
            new GestureScopeMapping(TextEditorGestureScope.TextEditorScope, new MultiKeyGesture(Key.Return, ModifierKeys.Control | ModifierKeys.Shift))
        });
    }
}