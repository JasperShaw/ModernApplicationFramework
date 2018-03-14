using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces.Command;

namespace ModernApplicationFramework.EditorBase.Commands
{
    [Export(typeof(ICommandHandler))]
    public class RecentFilesListCommandHandler :ICommandListHandler<RecentFilesListDefinition>
    {
        private readonly IDockingHostViewModel _shell;

        [ImportingConstructor]
        public RecentFilesListCommandHandler(IDockingHostViewModel shell)
        {
            _shell = shell;
        }

        public void Populate(Command command, List<CommandDefinitionBase> commands)
        {
            var activeFiles = _shell.LayoutItems.Count;
            var maxFiles = IoC.Get<EnvironmentGeneralOptions>().WindowListItems;
            var fileCount = activeFiles < maxFiles ? activeFiles : maxFiles;

            for (var i = 0; i < fileCount; i++)
            {
                var document = _shell.LayoutItems[i];

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
            public override UICommand Command { get; }

            public override MultiKeyGesture DefaultKeyGesture => null;
            public override GestureScope DefaultGestureScope => null;

            public override string Name => string.Empty;
            public override string NameUnlocalized => string.Empty;
            public override string Text { get; }
            public override string ToolTip => string.Empty;
            public override Uri IconSource => null;
            public override string IconId => null;
            public override CommandCategory Category => null;
            public override Guid Id => new Guid("{5DF9C8CC-9916-4E62-B186-4463D41F2705}");

            public ShowSelectedDocumentCommandDefinition(string name)
            {
                Text = name;
                Command = new UICommand(ShowSelectedItem, CanShowSelectedItem);
            }

            private bool CanShowSelectedItem()
            {
                return CommandParamenter is ILayoutItem;
            }

            private void ShowSelectedItem()
            {
                IoC.Get<IDockingHostViewModel>().OpenLayoutItem((ILayoutItem)CommandParamenter);
            }
        }
    }
}
