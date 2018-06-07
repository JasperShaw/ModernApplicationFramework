using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.EditorBase.FileSupport;
using ModernApplicationFramework.EditorBase.Interfaces.Packages;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces.Command;

namespace ModernApplicationFramework.EditorBase.Commands
{
    [Export(typeof(ICommandHandler))]
    public class RecentFilesListCommandHandler :ICommandListHandler<RecentFilesListDefinition>
    {
        private readonly IMruFilePackage _mruFilePackage;

        [ImportingConstructor]
        public RecentFilesListCommandHandler(IMruFilePackage mruFilePackage)
        {
            _mruFilePackage = mruFilePackage;
        }

        public void Populate(Command command, List<CommandDefinitionBase> commands)
        {

            var items = _mruFilePackage.Manager.Items.Count;
            var maxMruFiles = IoC.Get<EnvironmentGeneralOptions>().MRUListItems;
            var itemCount = items < maxMruFiles ? items : maxMruFiles;

            if (itemCount == 0)
            {
                var definition = new OpenMruFileCommandDefinition(-1, CommandsResources.RecentFileListCommand_NoItems);
                commands.Add(definition);
                return;
            }

            for (var i = 0; i < itemCount; i++)
            {
                var item = _mruFilePackage.Manager.Items[i];
                var definition = new OpenMruFileCommandDefinition(i,$"&{i+1} {ShrinkPath(item.Path, 50)}")
                {
                    CommandParamenter = item
                };
                commands.Add(definition);
            }
        }

        public string ShrinkPath(string path, int maxLength)
        {
            if (path.Length <= maxLength)
                return path;
            var parts = new List<string>(path.Split('\\'));
            var start = parts[0] + @"\" + parts[1];
            parts.RemoveAt(1);
            parts.RemoveAt(0);
            var end = parts[parts.Count - 1];
            parts.RemoveAt(parts.Count - 1);

            parts.Insert(0, "...");
            while (parts.Count > 1 &&
                   start.Length + end.Length + parts.Sum(p => p.Length) + parts.Count > maxLength)
                parts.RemoveAt(parts.Count - 1);
            var mid = "";
            parts.ForEach(p => mid += @"\" + p + @"\");
            return start + mid + end;
        }

        private class OpenMruFileCommandDefinition : CommandDefinition
        {
            public override ICommand Command { get; }

            public override MultiKeyGesture DefaultKeyGesture => null;
            public override GestureScope DefaultGestureScope => null;

            public override string Name => string.Empty;
            public override string NameUnlocalized => string.Empty;

            private int Index { get; }

            public override string Text { get; }
            public override string ToolTip => string.Empty;
            public override Uri IconSource => null;
            public override string IconId => null;
            public override CommandCategory Category => null;
            public override Guid Id => new Guid("{5DF9C8CC-9916-4E62-B186-4463D41F2705}");

            public OpenMruFileCommandDefinition(int index, string name)
            {
                Index = index;
                Text = name;
                Command = new UICommand(OpenMruFile, CanOpenMruFile);
            }

            private bool CanOpenMruFile()
            {
                return CommandParamenter is FileSystemMruItem;
            }

            private void OpenMruFile()
            {
                IoC.Get<IMruFilePackage>().Manager.OpenItem(Index);
            }
        }
    }
}
