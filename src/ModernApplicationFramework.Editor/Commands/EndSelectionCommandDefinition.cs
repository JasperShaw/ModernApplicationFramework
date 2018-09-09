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
    internal class EndSelectionCommandDefinition : CommandDefinition
    {
        [ImportingConstructor]
        public EndSelectionCommandDefinition() : base(new TextEditCommand(MafConstants.EditorCommands.EndExt))
        {
        }

        public override string NameUnlocalized => "EndDocumentSelection";
        public override string Text => NameUnlocalized;
        public override string ToolTip => Text;
        public override CommandBarCategory Category => CommandBarCategories.EditCategory;
        public override Guid Id => new Guid("{BDBEAF00-1C7B-44A5-B5AB-A76805F2B3BE}");

        public override ReadOnlyCollection<GestureScopeMapping> DefaultGestureScopes =>
            new ReadOnlyCollection<GestureScopeMapping>(new[]
            {
                new GestureScopeMapping(TextEditorGestureScope.TextEditorScope,
                    new MultiKeyGesture(Key.End, ModifierKeys.Control | ModifierKeys.Shift))
            });
    }
}