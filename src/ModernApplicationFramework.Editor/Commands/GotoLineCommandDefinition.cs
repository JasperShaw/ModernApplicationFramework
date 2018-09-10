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
    internal class GotoLineCommandDefinition : CommandDefinition
    {
        [ImportingConstructor]
        public GotoLineCommandDefinition() : base(new TextEditCommand(MafConstants.EditorCommands.GotoLine))
        {
        }

        public override string NameUnlocalized => "GotoLine";
        public override string Text => NameUnlocalized;
        public override string ToolTip => Text;
        public override CommandBarCategory Category => CommandBarCategories.EditCategory;
        public override Guid Id => new Guid("{0D098AF4-1077-4EBD-94C5-C601C0361561}");

        public override ReadOnlyCollection<GestureScopeMapping> DefaultGestureScopes => new ReadOnlyCollection<GestureScopeMapping>(new[]
        {
            new GestureScopeMapping(TextEditorGestureScope.TextEditorScope, new MultiKeyGesture(Key.G, ModifierKeys.Control))
        });
    }

    [Export(typeof(CommandBarItemDefinition))]
    internal class GotoBraceCommandDefinition : CommandDefinition
    {
        [ImportingConstructor]
        public GotoBraceCommandDefinition() : base(new TextEditCommand(MafConstants.EditorCommands.GotoBrace))
        {
        }

        public override string NameUnlocalized => "Goto Brace";
        public override string Text => NameUnlocalized;
        public override string ToolTip => Text;
        public override CommandBarCategory Category => CommandBarCategories.EditCategory;
        public override Guid Id => new Guid("{795CCD9B-F426-4E73-A059-6563A3839616}");

        public override ReadOnlyCollection<GestureScopeMapping> DefaultGestureScopes => new ReadOnlyCollection<GestureScopeMapping>(new[]
        {
            new GestureScopeMapping(TextEditorGestureScope.TextEditorScope, new MultiKeyGesture(Key.Oem6, ModifierKeys.Control))
        });
    }

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