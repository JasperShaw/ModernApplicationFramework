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
    internal class TransposeLineCommandDefinition : CommandDefinition
    {
        [ImportingConstructor]
        public TransposeLineCommandDefinition() : base(new TextEditCommand(MafConstants.EditorCommands.TransposeLine))
        {
        }

        public override string NameUnlocalized => "Transpose Line";
        public override string Text => NameUnlocalized;
        public override string ToolTip => Text;
        public override CommandBarCategory Category => CommandBarCategories.EditCategory;
        public override Guid Id => new Guid("{53263459-2CAE-4174-B0F0-FE16F5B0F85E}");

        public override ReadOnlyCollection<GestureScopeMapping> DefaultGestureScopes => new ReadOnlyCollection<GestureScopeMapping>(new[]
        {
            new GestureScopeMapping(TextEditorGestureScope.TextEditorScope, new MultiKeyGesture(Key.T, ModifierKeys.Alt | ModifierKeys.Shift))
        });
    }
}