using System;
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
    public class DeleteCommandDefinition : CommandDefinition
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
        public override MultiKeyGesture DefaultKeyGesture { get; }
        public override GestureScope DefaultGestureScope { get; }

        public override ICommand Command { get; }

        public DeleteCommandDefinition()
        {
            //Apparently even Routed Commands like the Editing Commands need to get wrapped so keybinding changes will be recognized
            Command = new UICommand(Delete, CanDelete);
            DefaultKeyGesture = new MultiKeyGesture(Key.Delete);
            DefaultGestureScope = GestureScopes.GlobalGestureScope;
        }

        private bool CanDelete()
        {
            return EditingCommands.Delete.CanExecute(null, null);
        }

        private static void Delete()
        {
            EditingCommands.Delete.Execute(null, null);
        }
    }
}
