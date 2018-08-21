using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Editor.Commanding;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Editor.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    internal class LeftCommandDefinition : CommandDefinition
    {
        [ImportingConstructor]
        public LeftCommandDefinition([Import] IActiveTextViewState activeTextViewState)
        {
            Command = new TextEditCommand(MafConstants.EditorCommands.Left);
        }

        public override string NameUnlocalized => "CharLeft";
        public override string Text => NameUnlocalized;
        public override string ToolTip => Text;
        public override CommandCategory Category => CommandCategories.EditCommandCategory;
        public override Guid Id => new Guid("{3CBBD74E-9DA9-4D4D-A0BB-77DEB8A6636B}");

        public override ReadOnlyCollection<GestureScopeMapping> DefaultGestureScopes => new ReadOnlyCollection<GestureScopeMapping>(new[]
        {
            new GestureScopeMapping(GestureScopes.GlobalGestureScope, new MultiKeyGesture(Key.Left)),
            new GestureScopeMapping(GestureScopes.GlobalGestureScope, new MultiKeyGesture(Key.Right))
        });
    }
}
