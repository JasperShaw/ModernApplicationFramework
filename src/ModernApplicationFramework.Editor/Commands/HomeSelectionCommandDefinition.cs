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
    internal class HomeSelectionCommandDefinition : CommandDefinition
    {
        [ImportingConstructor]
        public HomeSelectionCommandDefinition() : base(new TextEditCommand(MafConstants.EditorCommands.HomeExt))
        {
        }

        public override string NameUnlocalized => "StartDocumentSelection";
        public override string Text => NameUnlocalized;
        public override string ToolTip => Text;
        public override CommandBarCategory Category => CommandBarCategories.EditCategory;
        public override Guid Id => new Guid("{F0B9B24C-863A-48F3-B115-B481E9B1CBBF}");

        public override ReadOnlyCollection<GestureScopeMapping> DefaultGestureScopes =>
            new ReadOnlyCollection<GestureScopeMapping>(new[]
            {
                new GestureScopeMapping(TextEditorGestureScope.TextEditorScope,
                    new MultiKeyGesture(Key.Home, ModifierKeys.Control | ModifierKeys.Shift))
            });
    }
}