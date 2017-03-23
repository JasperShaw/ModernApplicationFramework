using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Interfaces.Command;

namespace ModernApplicationFramework.Extended.Commands
{
    [Export(typeof(ICommandHandler))]
    public class SwitchToDocumentListCommandHandler : ICommandListHandler<SwitchToDocumentCommandListDefinition>
    {
        private readonly IDockingHostViewModel _shell;

        [ImportingConstructor]
        public SwitchToDocumentListCommandHandler(IDockingHostViewModel shell)
        {
            _shell = shell;
        }

        public void Populate(Command command, List<CommandDefinition> commands)
        {
            for (var i = 0; i < _shell.Documents.Count; i++)
            {
                var document = _shell.Documents[i];

                var d = new SimpleCommandDefinition($"_{i + 1} {document.DisplayName}") {CommandParamenter = document};

                commands.Add(d);
            }
        }
    }



    public class SimpleCommandDefinition : CommandDefinition
    {

        public SimpleCommandDefinition(string name)
        {
            Name = name;
            Command = new CommandWrapper(Test, CanTest);
        }

        public override ICommand Command { get; }

        private bool CanTest()
        {
            return true;
        }

        private void Test()
        {
            IoC.Get<IDockingHostViewModel>().OpenDocument((ILayoutItem)CommandParamenter);
        }

        public override bool CanShowInMenu { get; }
        public override bool CanShowInToolbar { get; }
        public override string IconId { get; }
        public override Uri IconSource { get; }
        public override string Name { get; }
        public override string Text { get; }
        public override string ToolTip { get; }
    }
}
