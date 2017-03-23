using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.CommandBase;

namespace ModernApplicationFramework.Extended.Commands
{
    [Export(typeof(CommandDefinition))]
    public class SwitchToDocumentCommandListDefinition : CommandListDefinition
    {
        public override bool CanShowInMenu => false;
        public override bool CanShowInToolbar => false;
        public override string Name => "Window.SwitchToDocument";
    }
}
