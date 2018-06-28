using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Documents;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Extended.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(DeleteCommandDefinition))]
    public class DeleteCommandDefinition : CommandDefinition<IDeleteCommand>
    {
        public override string NameUnlocalized => "Delete";
        public override string Text => NameUnlocalized;
        public override string ToolTip => Text;

        public override Uri IconSource =>
            new Uri("/ModernApplicationFramework.Extended;component/Resources/Icons/Delete_16x.xaml",
                UriKind.RelativeOrAbsolute);

        public override string IconId => "DeleteIcon";
        public override CommandCategory Category => CommandCategories.EditCommandCategory;
        public override Guid Id => new Guid("{667CA2DA-8DBD-4D93-8167-007A38A82A2B}");
        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures { get; }
        public override GestureScope DefaultGestureScope { get; }

        public DeleteCommandDefinition()
        {
            DefaultKeyGestures = new []{new MultiKeyGesture(Key.Delete)};
            DefaultGestureScope = GestureScopes.GlobalGestureScope;
        }
    }

    public interface IDeleteCommand : ICommandDefinitionCommand
    {

    }

    [Export(typeof(IDeleteCommand))]
    internal class DeleteCommand : CommandDefinitionCommand, IDeleteCommand
    {
        protected override bool OnCanExecute(object parameter)
        {
            return EditingCommands.Delete.CanExecute(parameter, null);
        }

        protected override void OnExecute(object parameter)
        {
            EditingCommands.Delete.Execute(parameter, null);
        }
    }
}
