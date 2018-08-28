using System.Collections.Generic;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces.Services;

namespace ModernApplicationFramework.Extended.CommandBar.CommandDefinitions
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

        public void Populate(Command command, List<CommandItemDefinitionBase> commands)
        {
            var activeFiles = _shell.LayoutItems.Count;
            var maxFiles = IoC.Get<EnvironmentGeneralOptions>().WindowListItems;
            var fileCount = activeFiles < maxFiles ? activeFiles : maxFiles;

            for (var i = 0; i < fileCount; i++)
            {
                var document = _shell.LayoutItems[i];

                var definition =
                    new CommandListHandlerDefinition($"&{i + 1} {document.DisplayName}",
                        new ShowSelectedDocumentCommand(document));
                commands.Add(definition);
            }
        }

        private class ShowSelectedDocumentCommand : CommandDefinitionCommand
        {
            public ShowSelectedDocumentCommand(object args) : base(args)
            {
                if (args is ILayoutItem layoutItem && layoutItem.IsActive)
                    Checked = true;
                else
                    Checked = false;
            }

            protected override bool OnCanExecute(object parameter)
            {
                return parameter is ILayoutItem;
            }

            protected override void OnExecute(object parameter)
            {
                IoC.Get<IDockingHostViewModel>().OpenLayoutItem((ILayoutItem) parameter);
            }
        }
    }
}