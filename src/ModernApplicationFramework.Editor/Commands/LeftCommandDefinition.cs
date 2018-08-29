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
    internal class LeftCommandDefinition : CommandDefinition
    {
        [ImportingConstructor]
        public LeftCommandDefinition() : base(new TextEditCommand(MafConstants.EditorCommands.Left))
        {
        }

        public override string NameUnlocalized => "CharLeft";
        public override string Text => NameUnlocalized;
        public override string ToolTip => Text;
        public override CommandBarCategory Category => CommandBarCategories.EditCategory;
        public override Guid Id => new Guid("{3CBBD74E-9DA9-4D4D-A0BB-77DEB8A6636B}");

        public override ReadOnlyCollection<GestureScopeMapping> DefaultGestureScopes => new ReadOnlyCollection<GestureScopeMapping>(new[]
        {
            new GestureScopeMapping(TextEditorGestureScope.TextEditorScope, new MultiKeyGesture(Key.Left)),
            new GestureScopeMapping(TextEditorGestureScope.OutputGestureScope, new MultiKeyGesture(Key.Right))
        });
    }
}
