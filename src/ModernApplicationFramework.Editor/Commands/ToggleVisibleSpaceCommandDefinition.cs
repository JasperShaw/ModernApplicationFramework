using System;
using System.Collections.Generic;
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
    internal class ToggleVisibleSpaceCommandDefinition : CommandDefinition
    {
        [ImportingConstructor]
        public ToggleVisibleSpaceCommandDefinition() : base(new TextEditCommand(MafConstants.EditorCommands.ToggleVisibleSpace))
        {

        }

        public override string NameUnlocalized => "ToggleVisibleSpace";
        public override string Text => NameUnlocalized;
        public override string ToolTip => Text;
        public override CommandBarCategory Category => CommandBarCategories.EditCategory;
        public override Guid Id => new Guid("{A4189660-2B3C-4B94-86CB-C8D44BCB0814}");
        public override bool Checkable => true;

        public override ReadOnlyCollection<GestureScopeMapping> DefaultGestureScopes => new ReadOnlyCollection<GestureScopeMapping>(new[]
        {
            new GestureScopeMapping(TextEditorGestureScope.TextEditorScope, new MultiKeyGesture(new List<KeySequence>
            {
                new KeySequence(ModifierKeys.Control, Key.R),
                new KeySequence(ModifierKeys.Control, Key.W),
            }))
        });
    }
}