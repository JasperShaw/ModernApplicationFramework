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
    internal class SelectCurretnWordCommandDefinition : CommandDefinition
    {
        [ImportingConstructor]
        public SelectCurretnWordCommandDefinition() : base(new TextEditCommand(MafConstants.EditorCommands.SelectCurrentWord))
        {
        }

        public override string NameUnlocalized => "SelectCurrentWord";
        public override string Text => NameUnlocalized;
        public override string ToolTip => Text;
        public override CommandBarCategory Category => CommandBarCategories.EditCategory;
        public override Guid Id => new Guid("{C938EB8B-7163-4695-BEAD-655EF6D04096}");

        public override ReadOnlyCollection<GestureScopeMapping> DefaultGestureScopes => new ReadOnlyCollection<GestureScopeMapping>(new[]
        {
            new GestureScopeMapping(TextEditorGestureScope.TextEditorScope, new MultiKeyGesture(Key.W, ModifierKeys.Control))
        });
    }
}