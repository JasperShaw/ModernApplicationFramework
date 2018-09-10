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
    internal class GotoBraceExtendCommandDefinition : CommandDefinition
    {
        [ImportingConstructor]
        public GotoBraceExtendCommandDefinition() : base(new TextEditCommand(MafConstants.EditorCommands.GotoBraceExt))
        {
        }

        public override string NameUnlocalized => "Goto Brace Extend";
        public override string Text => NameUnlocalized;
        public override string ToolTip => Text;
        public override CommandBarCategory Category => CommandBarCategories.EditCategory;
        public override Guid Id => new Guid("{CFEE8D69-67E4-4263-A341-31C03D2478D2}");

        public override ReadOnlyCollection<GestureScopeMapping> DefaultGestureScopes => new ReadOnlyCollection<GestureScopeMapping>(new[]
        {
            new GestureScopeMapping(TextEditorGestureScope.TextEditorScope, new MultiKeyGesture(Key.Oem6, ModifierKeys.Control | ModifierKeys.Shift))
        });
    }
}