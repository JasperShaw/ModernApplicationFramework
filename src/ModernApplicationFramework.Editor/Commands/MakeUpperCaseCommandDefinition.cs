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
    internal class MakeUpperCaseCommandDefinition : CommandDefinition
    {
        [ImportingConstructor]
        public MakeUpperCaseCommandDefinition() : base(new TextEditCommand(MafConstants.EditorCommands.MakeUpperCase))
        {
        }

        public override string NameUnlocalized => "Make Upper Case";
        public override string Text => NameUnlocalized;
        public override string ToolTip => Text;
        public override CommandBarCategory Category => CommandBarCategories.EditCategory;
        public override Guid Id => new Guid("{B36C6E3F-2D7A-437D-8FC0-E39D319004E7}");

        public override ReadOnlyCollection<GestureScopeMapping> DefaultGestureScopes => new ReadOnlyCollection<GestureScopeMapping>(new[]
        {
            new GestureScopeMapping(TextEditorGestureScope.TextEditorScope, new MultiKeyGesture(Key.U, ModifierKeys.Control | ModifierKeys.Shift))
        });
    }
}