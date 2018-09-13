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
    internal class DeleteWordLeftCommandDefinition : CommandDefinition
    {
        [ImportingConstructor]
        public DeleteWordLeftCommandDefinition() : base(new TextEditCommand(MafConstants.EditorCommands.DeleteWordLeft))
        {
        }

        public override string NameUnlocalized => "DeleteToStart";
        public override string Text => NameUnlocalized;
        public override string ToolTip => Text;
        public override CommandBarCategory Category => CommandBarCategories.EditCategory;
        public override Guid Id => new Guid("{2F6451D3-2D6B-4CD2-BF03-34AC8B4EB338}");

        public override ReadOnlyCollection<GestureScopeMapping> DefaultGestureScopes => new ReadOnlyCollection<GestureScopeMapping>(new[]
        {
            new GestureScopeMapping(TextEditorGestureScope.TextEditorScope, new MultiKeyGesture(Key.Back, ModifierKeys.Control))
        });
    }
}