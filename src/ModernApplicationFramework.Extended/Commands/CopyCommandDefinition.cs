using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Extended.Clipboard;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Extended.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    public class CopyCommandDefinition : CommandDefinition<ICopyCommand>
    {
        public override string NameUnlocalized => "Copy";
        public override string Text => "Copy";
        public override string ToolTip => Text;
        public override Uri IconSource => null;
        public override string IconId => null;
        public override CommandCategory Category => CommandCategories.EditCommandCategory;
        public override Guid Id => new Guid("{E98D986F-AACB-4BC7-A60B-E758CA847BA9}");
        public override MultiKeyGesture DefaultKeyGesture { get; }
        public override GestureScope DefaultGestureScope { get; }

        public CopyCommandDefinition()
        {
            DefaultKeyGesture = new MultiKeyGesture(Key.C, ModifierKeys.Control);
            DefaultGestureScope = GestureScopes.GlobalGestureScope;
        }
    }

    [Export(typeof(ICopyCommand))]
    public class CopyCommand : CommandDefinitionCommand, ICopyCommand
    {
        protected override bool OnCanExecute(object parameter)
        {
            return ApplicationCommands.Copy.CanExecute(parameter, null);
        }

        protected override void OnExecute(object parameter)
        {
            ApplicationCommands.Copy.Execute(parameter, null);
            CopyCutWatcher.PushClipboard();
        }
    }

    public interface ICopyCommand : ICommandDefinitionCommand
    {
    }
}
