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
    public class PasteCommandDefinition : CommandDefinition<IPasteCommand>
    {
        public override string NameUnlocalized => "Paste";
        public override string Text => "Paste";
        public override string ToolTip => Text;
        public override Uri IconSource => new Uri("/ModernApplicationFramework.Extended;component/Resources/Icons/Paste_16x.xaml",
            UriKind.RelativeOrAbsolute);
        public override string IconId => "PasteIcon";
        public override CommandCategory Category => CommandCategories.EditCommandCategory;
        public override Guid Id => new Guid("{7820A125-F085-4338-81B3-22985BE24B55}");
        public override MultiKeyGesture DefaultKeyGesture => new MultiKeyGesture(Key.V, ModifierKeys.Control);
        public override GestureScope DefaultGestureScope => GestureScopes.GlobalGestureScope;
    }

    public interface IPasteCommand : ICommandDefinitionCommand
    {
    }

    [Export(typeof(IPasteCommand))]
    public class PasteCommand : CommandDefinitionCommand, IPasteCommand
    {
        protected override bool OnCanExecute(object parameter)
        {
            return ApplicationCommands.Paste.CanExecute(parameter, null);
        }

        protected override void OnExecute(object parameter)
        {
            ApplicationCommands.Paste.Execute(parameter, null);
            CopyCutWatcher.PushClipboard(ClipboardPushOption.Paste);
        }
    }
}
