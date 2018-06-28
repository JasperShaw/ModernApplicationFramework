using System;
using System.Collections.Generic;
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
    public class CutCommandDefinition : CommandDefinition<ICutCommand>
    {
        public override string NameUnlocalized => "Cut";
        public override string Text => "Cut";
        public override string ToolTip => Text;
        public override Uri IconSource => new Uri("/ModernApplicationFramework.Extended;component/Resources/Icons/Cut_16x.xaml",
            UriKind.RelativeOrAbsolute);
        public override string IconId => "CutIcon";
        public override CommandCategory Category => CommandCategories.EditCommandCategory;
        public override Guid Id => new Guid("{E0C9B4B8-C72E-43C4-AE1C-1FF00D3AB4CA}");
        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures => new []{new MultiKeyGesture(Key.X, ModifierKeys.Control)};
        public override GestureScope DefaultGestureScope => GestureScopes.GlobalGestureScope;
    }

    public interface ICutCommand : ICommandDefinitionCommand
    {
    }

    [Export(typeof(ICutCommand))]
    public class CutCommand : CommandDefinitionCommand, ICutCommand
    {
        protected override bool OnCanExecute(object parameter)
        {
            return ApplicationCommands.Cut.CanExecute(parameter, null);
        }

        protected override void OnExecute(object parameter)
        {
            ApplicationCommands.Cut.Execute(parameter, null);
            CopyCutWatcher.PushClipboard(ClipboardPushOption.Cut);
        }
    }
}
