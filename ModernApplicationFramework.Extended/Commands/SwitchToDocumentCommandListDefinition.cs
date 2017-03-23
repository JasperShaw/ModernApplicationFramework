using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.CommandBase;

namespace ModernApplicationFramework.Extended.Commands
{
    [Export(typeof(DefinitionBase))]
    public class SwitchToDocumentCommandListDefinition : CommandListDefinition
    {
        public override string Name => "Window.SwitchToDocument";
    }
}
