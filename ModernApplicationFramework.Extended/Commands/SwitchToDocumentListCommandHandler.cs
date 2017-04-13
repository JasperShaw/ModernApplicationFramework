using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.Command;
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

        public void Populate(Command command, List<DefinitionBase> commands)
        {
            for (var i = 0; i < _shell.Documents.Count; i++)
            {
                var document = _shell.Documents[i];

                var definition =
                    new ShowSelectedDocumentCommandDefinition($"&{i + 1} {document.DisplayName}")
                    {
                        CommandParamenter = document
                    };
                if (document.IsActive)
                    definition.IsChecked = true;

                commands.Add(definition);
            }
        }

        private class ShowSelectedDocumentCommandDefinition : CommandDefinition
        {
            public override ICommand Command { get; }

            public override string Name => string.Empty;
            public override string Text { get; }
            public override string ToolTip => string.Empty;
            public override Uri IconSource => null;
            public override string IconId => null;
            public override CommandCategory Category => null;

            public ShowSelectedDocumentCommandDefinition(string name)
            {
                Text = name;
                Command = new MultiKeyGestureCommandWrapper(ShowSelectedItem, CanShowSelectedItem);
            }

            private bool CanShowSelectedItem()
            {
                return CommandParamenter is ILayoutItem;
            }

            private void ShowSelectedItem()
            {
                IoC.Get<IDockingHostViewModel>().OpenDocument((ILayoutItem) CommandParamenter);
            }
        }
    }
}