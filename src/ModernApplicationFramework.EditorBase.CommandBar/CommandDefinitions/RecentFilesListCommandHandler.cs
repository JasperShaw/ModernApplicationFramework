using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.EditorBase.CommandBar.Resources;
using ModernApplicationFramework.EditorBase.FileSupport;
using ModernApplicationFramework.EditorBase.Interfaces.Packages;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces.Services;

namespace ModernApplicationFramework.EditorBase.CommandBar.CommandDefinitions
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

        public void Populate(Command command, List<CommandItemDefinitionBase> commands)
        {

            var items = _mruFilePackage.Manager.Items.Count;
            var maxMruFiles = IoC.Get<EnvironmentGeneralOptions>().MRUListItems;
            var itemCount = items < maxMruFiles ? items : maxMruFiles;

            if (itemCount == 0)
            {
                var args = new MruCommandParameter(-1, null);
                var definition = new CommandListHandlerDefinition(CommandsResources.RecentFileListCommand_NoItems, new OpenMruFileCommand(args));
                commands.Add(definition);
                return;
            }

            for (var i = 0; i < itemCount; i++)
            {
                var item = _mruFilePackage.Manager.Items[i];
                var args = new MruCommandParameter(i, item);
                var definition = new CommandListHandlerDefinition($"&{i + 1} {ShrinkPath(item.Path, 50)}", new OpenMruFileCommand(args));
                commands.Add(definition);
            }
        }

        private string ShrinkPath(string path, int maxLength)
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

        private class OpenMruFileCommand : CommandDefinitionCommand
        {
            public OpenMruFileCommand(object args) : base(args)
            {
            }

            protected override bool OnCanExecute(object parameter)
            {
                if (!(parameter is MruCommandParameter mruCommandParameter))
                    return false;
                return mruCommandParameter.Item != null;
            }

            protected override void OnExecute(object parameter)
            {
                IoC.Get<IMruFilePackage>().Manager.OpenItem(((MruCommandParameter)parameter).Index);
            }
        }

        private struct MruCommandParameter
        {
            public readonly int Index;
            public readonly FileSystemMruItem Item;

            public MruCommandParameter(int index, FileSystemMruItem item)
            {
                Index = index;
                Item = item;
            }
        }
    }
}
