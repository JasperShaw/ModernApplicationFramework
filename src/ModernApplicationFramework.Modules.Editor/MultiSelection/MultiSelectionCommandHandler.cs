using System.ComponentModel.Composition;
using ModernApplicationFramework.Modules.Editor.Text;
using ModernApplicationFramework.Text.Ui.Commanding;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Editor.Commanding.Commands;
using ModernApplicationFramework.Text.Ui.Text;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Modules.Editor.MultiSelection
{
    [Export(typeof(ITextEditCommand))]
    [Name("MultiSelectionCommandHandler")]
    [ContentType("any")]
    [TextViewRole("INTERACTIVE")]
    internal sealed class MultiSelectionCommandHandler : ITextEditCommand<EscapeKeyCommandArgs>
    {
        [Import]
        internal IMultiSelectionBrokerFactory MultiSelectionBrokerFactoryService { get; set; }

        //TODO: search
        //[Import]
        //internal ITextSearchNavigatorFactoryService TextSearchNavigatorFactoryService { get; set; }

        public CommandState GetCommandState(EscapeKeyCommandArgs args)
        {
            return CommandState.Available;
        }

        public bool ExecuteCommand(EscapeKeyCommandArgs args, CommandExecutionContext executionContext)
        {
            var multiSelectionBroker = args.TextView.GetMultiSelectionBroker();
            if (multiSelectionBroker.IsBoxSelection)
            {
                using (multiSelectionBroker.BeginBatchOperation())
                {
                    multiSelectionBroker.ClearSecondarySelections();
                    multiSelectionBroker.PerformActionOnAllSelections(PredefinedSelectionTransformations.ClearSelection);
                    return true;
                }
            }
            if (multiSelectionBroker.HasMultipleSelections)
            {
                multiSelectionBroker.ClearSecondarySelections();
                return true;
            }

            if (multiSelectionBroker.PrimarySelection.IsEmpty)
                return false;
            multiSelectionBroker.PerformActionOnAllSelections(PredefinedSelectionTransformations.ClearSelection);
            return true;
        }
    }
}
