using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.EditorBase.FileSupport;
using ModernApplicationFramework.EditorBase.Interfaces.Services;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces.Commands;

namespace ModernApplicationFramework.EditorBase.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(NewFileCommandDefinition))]
    public class OpenFileCommandDefinition : CommandDefinition<IOpenFileCommand>
    {
        public override string NameUnlocalized => "Open File";
        public override string Name => "OpenFile";
        public override string Text => CommandsResources.OpenFileCommandText;
        public override string ToolTip => Text;
        public override Uri IconSource => new Uri("/ModernApplicationFramework.EditorBase;component/Resources/Icons/OpenFolder_16x.xaml",
            UriKind.RelativeOrAbsolute);
        public override string IconId => "OpenFileIcon";
        public override CommandCategory Category => CommandCategories.FileCommandCategory;
        public override Guid Id => new Guid("{47E7AF89-3733-4FBF-A3FA-E8AD5D5C693E}");
        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures { get; }
        public override GestureScope DefaultGestureScope { get; }

        public OpenFileCommandDefinition()
        {
            DefaultKeyGestures = new []{new MultiKeyGesture(Key.O, ModifierKeys.Control)};
            DefaultGestureScope = GestureScopes.GlobalGestureScope;
        }
    }

    public interface IOpenFileCommand : ICommandDefinitionCommand
    {
    }

    [Export(typeof(IOpenFileCommand))]
    internal class OpenFileCommand : CommandDefinitionCommand, IOpenFileCommand
    {
        protected override bool OnCanExecute(object parameter)
        {
            return true;
        }

        protected override void OnExecute(object parameter)
        {
            var arguments = FileService.Instance.ShowOpenFilesWithDialog();
            if (!arguments.Any())
                return;
            var service = IoC.Get<IOpenFileService>();
            foreach (var argument in arguments)
                service.OpenFile(argument);
        }
    }
}
