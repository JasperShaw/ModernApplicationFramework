using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using Caliburn.Micro;
using ModernApplicationFramework.Editor.Commanding;
using ModernApplicationFramework.Editor.Interop;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Ui.Commanding;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Editor.Commanding;
using Action = System.Action;

namespace ModernApplicationFramework.Editor.Implementation
{
    internal partial class CommandHandlerServiceAdapter : ICommandHandlerServiceAdapter
    {
        private readonly IEditorCommandHandlerService _commandHandlerService;
        private static Dictionary<ValueTuple<Guid, uint>, string> _commandBindings;
        public const int ExecuteInInteractiveWindow = 268;

        public CommandHandlerServiceAdapter(ITextView textView, ICommandTarget nextCommandTarget)
            : this(textView, null, nextCommandTarget)
        {
        }

        public CommandHandlerServiceAdapter(ITextView textView, ITextBuffer subjectBuffer,
            ICommandTarget nextCommandTarget)
        {
            var textView1 = textView;
            TextView = textView1 ?? throw new ArgumentNullException(nameof(textView));
            NextCommandTarget = nextCommandTarget;
            _commandHandlerService = IoC.Get<IEditorCommandHandlerServiceFactory>().GetService(TextView, subjectBuffer);
            InitializeCommandBindings();
        }

        private static void InitializeCommandBindings()
        {
            var list = IoC.GetAll<Lazy<CommandBindingDefinition, ICommandBindingMetadata>>();

            if (_commandBindings != null)
                return;
            _commandBindings = new Dictionary<ValueTuple<Guid, uint>, string>();
            foreach (var export in list)
            {
                var metadata = export.Metadata;
                if (metadata.CommandSet != null && metadata.CommandArgsType != null &&
                    (metadata.CommandId != null && metadata.CommandId.Length == metadata.CommandSet.Length) &&
                    metadata.CommandSet.Length == metadata.CommandArgsType.Length)
                {
                    for (var index = 0; index < export.Metadata.CommandSet.Length; ++index)
                    {
                        if (Guid.TryParse(metadata.CommandSet[index], out var result))
                            _commandBindings[new ValueTuple<Guid, uint>(result, metadata.CommandId[index])] =
                                metadata.CommandArgsType[index];
                    }
                }
            }
        }

        public ITextView TextView { get; }

        public ICommandTarget NextCommandTarget { get; }

        public int QueryStatus(Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        {
            var num = !(pguidCmdGroup == MafConstants.EditorCommandGroup)
                ? QueryCustomCommandStatus(pguidCmdGroup, commandCount, prgCmds, commandText)
                : QueryEditorCommandGroup(pguidCmdGroup, commandCount, prgCmds, commandText);
            if (num == 0)
                return num;
            if (NextCommandTarget == null)
                return -2147467259;
            return NextCommandTarget.QueryStatus(pguidCmdGroup, commandCount, prgCmds, commandText);
        }

        public int Exec(Guid pguidCmdGroup, uint nCmdId, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            if (nCmdexecopt == 65539U)
            {
                var nextCommandTarget = NextCommandTarget;
                if (nextCommandTarget == null)
                    return -2147467259;
                return nextCommandTarget.Exec(pguidCmdGroup, nCmdId, nCmdexecopt, pvaIn, pvaOut);
            }

            var nextTargetResult = new int?();
            var guidCmdGroup = pguidCmdGroup;
            var num2 = !(guidCmdGroup == MafConstants.EditorCommandGroup)
                ? ExecuteCustomCommand(guidCmdGroup, nCmdId, pvaIn, pvaOut, () =>
                {
                    var nextCommandTarget = NextCommandTarget;
                    nextTargetResult =
                        nextCommandTarget?.Exec(guidCmdGroup, nCmdId, nCmdexecopt, pvaIn, pvaOut) ??
                        -2147467259;
                })
                : ExecuteEditorCommand(guidCmdGroup, (MafConstants.EditorCommands) nCmdId, pvaIn, pvaOut, () =>
                {
                    var nextCommandTarget = NextCommandTarget;
                    nextTargetResult = nextCommandTarget?.Exec(guidCmdGroup, nCmdId, nCmdexecopt, pvaIn, pvaOut) ??
                                       -2147467259;
                });
            if (num2 == -2147221244)
            {
                var nextCommandTarget = NextCommandTarget;
                return nextCommandTarget?.Exec(guidCmdGroup, nCmdId, nCmdexecopt, pvaIn, pvaOut) ?? -2147467259;
            }

            if (nextTargetResult.HasValue)
            {
                var nullable1 = nextTargetResult;
                if ((nullable1.GetValueOrDefault() == 1 ? (nullable1.HasValue ? 1 : 0) : 0) == 0)
                {
                    var nullable2 = nextTargetResult;
                    if ((nullable2.GetValueOrDefault() == -2147467259 ? (nullable2.HasValue ? 1 : 0) : 0) == 0)
                        return num2;
                }

                return nextTargetResult.Value;
            }
            return num2;
        }

        private int ExecuteEditorCommand(Guid pguidCmdGroup, MafConstants.EditorCommands commandId, IntPtr pvaIn, IntPtr pvaOut,
            Action next)
        {
            switch (commandId)
            {
                case MafConstants.EditorCommands.TypeChar:
                    var forNativeVariant = (char)(ushort)Marshal.GetObjectForNativeVariant(pvaIn);
                    ExecuteTypeCharCommand(next, forNativeVariant);
                    return 0;
                case MafConstants.EditorCommands.Backspace:
                    ExecuteBackspaceKeyCommand(next);
                    return 0;
                case MafConstants.EditorCommands.Return:
                    ExecuteReturnKeyCommand(next);
                    return 0;
                case MafConstants.EditorCommands.Tab:
                    ExecuteTabKeyCommand(next);
                    return 0;
                case MafConstants.EditorCommands.BackTab:
                    ExecuteBackTabKeyCommand(next);
                    return 0;
                case MafConstants.EditorCommands.Delete:
                    ExecuteDeleteKeyCommand(next);
                    return 0;
                case MafConstants.EditorCommands.Left:
                    ExecuteLeftKeyCommand(next);
                    return 0;
                case MafConstants.EditorCommands.Right:
                    ExecuteRightKeyCommand(next);
                    return 0;
                case MafConstants.EditorCommands.Up:
                    ExecuteUpKeyCommand(next);
                    return 0;
                case MafConstants.EditorCommands.Down:
                    ExecuteDownKeyCommand(next);
                    return 0;
                case MafConstants.EditorCommands.Home:
                    ExecuteDocumentStartCommand(next);
                    return 0;
                case MafConstants.EditorCommands.End:
                    ExecuteDocumentEndCommand(next);
                    return 0;
                case MafConstants.EditorCommands.BeginOfLine:
                    ExecuteLineStartCommand(next);
                    return 0;
                case MafConstants.EditorCommands.BeginOfLineExt:
                    ExecuteLineStartExtendCommand(next);
                    return 0;
                case MafConstants.EditorCommands.EndOfLine:
                    ExecuteLineEndCommand(next);
                    return 0;
                case MafConstants.EditorCommands.EndOfLineExt:
                    ExecuteLineEndExtendCommand(next);
                    return 0;
                case MafConstants.EditorCommands.PageUp:
                    ExecutePageUpKeyCommand(next);
                    return 0;
                case MafConstants.EditorCommands.PageDown:
                    ExecutePageDownKeyCommand(next);
                    return 0;
                case MafConstants.EditorCommands.SelectAll:
                    ExecuteSelectAllCommand(next);
                    return 0;
                //case VSConstants.VSStd2KCmdID.CUT:
                //    ExecuteCutCommand(next);
                //    return 0;
                case MafConstants.EditorCommands.Copy:
                    ExecuteCopyCommand(next);
                    return 0;
                //case VSConstants.VSStd2KCmdID.PASTE:
                //    ExecutePasteCommand(next);
                //    return 0;
                //case VSConstants.VSStd2KCmdID.OPENLINEABOVE:
                //    ExecuteOpenLineAboveCommand(next);
                //    return 0;
                //case VSConstants.VSStd2KCmdID.OPENLINEBELOW:
                //    ExecuteOpenLineBelowCommand(next);
                //    return 0;
                //case VSConstants.VSStd2KCmdID.DELETEWORDRIGHT:
                //    ExecuteWordDeleteToEndCommand(next);
                //    return 0;
                //case VSConstants.VSStd2KCmdID.DELETEWORDLEFT:
                //    ExecuteWordDeleteToStartCommand(next);
                //    return 0;
                //case VSConstants.VSStd2KCmdID.COMMENTBLOCK:
                //case VSConstants.VSStd2KCmdID.COMMENT_BLOCK:
                //    ExecuteCommentSelectionCommand(next);
                //    return 0;
                //case VSConstants.VSStd2KCmdID.UNCOMMENTBLOCK:
                //case VSConstants.VSStd2KCmdID.UNCOMMENT_BLOCK:
                //    ExecuteUncommentSelectionCommand(next);
                //    return 0;
                //case VSConstants.VSStd2KCmdID.CANCEL:
                //    ExecuteEscapeKeyCommand(next);
                //    return 0;
                //case VSConstants.VSStd2KCmdID.PARAMINFO:
                //    ExecuteInvokeSignatureHelpCommand(next);
                //    return 0;
                //case VSConstants.VSStd2KCmdID.COMPLETEWORD:
                //    ExecuteCommitUniqueCompletionListItemCommand(next);
                //    return 0;
                //case VSConstants.VSStd2KCmdID.SHOWMEMBERLIST:
                //    ExecuteInvokeCompletionListCommand(next);
                //    return 0;
                //case VSConstants.VSStd2KCmdID.FORMATSELECTION:
                //    ExecuteFormatSelectionCommand(next);
                //    return 0;
                //case VSConstants.VSStd2KCmdID.QUICKINFO:
                //    ExecuteInvokeQuickInfoCommand(next);
                //    return 0;
                //case VSConstants.VSStd2KCmdID.FORMATDOCUMENT:
                //    ExecuteFormatDocumentCommand(next);
                //    return 0;
                //case VSConstants.VSStd2KCmdID.OUTLN_START_AUTOHIDING:
                //    ExecuteStartAutomaticOutliningCommand(next);
                //    return 0;
                //case VSConstants.VSStd2KCmdID.INSERTSNIPPET:
                //    ExecuteInsertSnippetCommand(next);
                //    return 0;
                //case VSConstants.VSStd2KCmdID.ECMD_NEXTMETHOD:
                //    ExecuteGoToNextMemberCommand(next);
                //    return 0;
                //case VSConstants.VSStd2KCmdID.ECMD_PREVMETHOD:
                //    ExecuteGoToPreviousMemberCommand(next);
                //    return 0;
                //case VSConstants.VSStd2KCmdID.ECMD_INSERTCOMMENT:
                //    ExecuteInsertCommentCommand(next);
                //    return 0;
                //case VSConstants.VSStd2KCmdID.EXPANDSELECTION:
                //    ExecuteExpandSelectionCommand(next);
                //    return 0;
                //case VSConstants.VSStd2KCmdID.COLLAPSESELECTION:
                //    ExecuteContractSelectionCommand(next);
                //    return 0;
                //case VSConstants.VSStd2KCmdID.RENAME:
                //    ExecuteRenameCommand(next);
                //    return 0;
                //case VSConstants.VSStd2KCmdID.EXTRACTMETHOD:
                //    ExecuteExtractMethodCommand(next);
                //    return 0;
                //case VSConstants.VSStd2KCmdID.ENCAPSULATEFIELD:
                //    ExecuteEncapsulateFieldCommand(next);
                //    return 0;
                //case VSConstants.VSStd2KCmdID.EXTRACTINTERFACE:
                //    ExecuteExtractInterfaceCommand(next);
                //    return 0;
                //case VSConstants.VSStd2KCmdID.REMOVEPARAMETERS:
                //    ExecuteRemoveParametersCommand(next);
                //    return 0;
                //case VSConstants.VSStd2KCmdID.REORDERPARAMETERS:
                //    ExecuteReorderParametersCommand(next);
                //    return 0;
                //case VSConstants.VSStd2KCmdID.SURROUNDWITH:
                //    ExecuteSurroundWithCommand(next);
                //    return 0;
                //case VSConstants.VSStd2KCmdID.ViewCallHierarchy:
                //    ExecuteViewCallHierarchyCommand(next);
                //    return 0;
                //case VSConstants.VSStd2KCmdID.ToggleConsumeFirstCompletionMode:
                //    ExecuteToggleCompletionModeCommand(next);
                //    return 0;
                //case VSConstants.VSStd2KCmdID.PROJSETTINGS | VSConstants.VSStd2KCmdID.CallBrowser3ShowFullNames:
                //    ExecuteNavigateToNextHighlightedReferenceCommand(next);
                //    return 0;
                //case VSConstants.VSStd2KCmdID.LINKONLY | VSConstants.VSStd2KCmdID.CallBrowser3ShowFullNames:
                //    ExecuteNavigateToPreviousHighlightedReferenceCommand(next);
                //    return 0;
                default:
                    return -2147221244;
            }
        }

        private int ExecuteCustomCommand(Guid pguidCmdGroup, uint commandId, IntPtr pvaIn, IntPtr pvaOut,
            Action next)
        {
            var key = new ValueTuple<Guid, uint>(pguidCmdGroup, commandId);
            if (_commandBindings.TryGetValue(key, out var typeName))
            {
                var type = Type.GetType(typeName, false);
                if (type != null)
                {
                    GetType().GetMethod(nameof(ExecuteCustomCommandHelper),
                        BindingFlags.Instance | BindingFlags.NonPublic).MakeGenericMethod(type).Invoke(this,
                        new object[]
                        {
                            type,
                            next
                        });
                    return 0;
                }
            }

            return -2147221244;
        }

        private T TryCreateCustomCommandArgs<T>(Type commandArgsType, ITextView textView, ITextBuffer buffer)
        {
            return EditorParts.GuardedOperations.CallExtensionPoint(this,
                () => (T) Activator.CreateInstance(commandArgsType, textView, buffer), default);
        }

        private void ExecuteCustomCommandHelper<T>(Type commandArgsType, Action nextCommandHandler)
            where T : EditorCommandArgs
        {
            _commandHandlerService.Execute(
                (view, buffer) => TryCreateCustomCommandArgs<T>(commandArgsType, view, buffer), nextCommandHandler);
        }

        private int QueryEditorCommandGroup(Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds,
            IntPtr commandText)
        {
            switch (prgCmds[0].cmdID)
            {
                case (uint)MafConstants.EditorCommands.Backspace:
                    return QueryBackspaceKeyStatus(pguidCmdGroup, commandCount, prgCmds, commandText);
                case (uint)MafConstants.EditorCommands.Return:
                    return QueryReturnKeyStatus(pguidCmdGroup, commandCount, prgCmds, commandText);
                case (uint)MafConstants.EditorCommands.Tab:
                    return QueryTabKeyStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                case (uint)MafConstants.EditorCommands.BackTab:
                    return QueryBackTabKeyStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                case (uint)MafConstants.EditorCommands.Delete:
                    return QueryDeleteKeyStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                case (uint) MafConstants.EditorCommands.Left:
                    return QueryLeftKeyStatus(pguidCmdGroup, commandCount, prgCmds, commandText);
                case (uint) MafConstants.EditorCommands.Right:
                    return QueryRightKeyStatus(pguidCmdGroup, commandCount, prgCmds, commandText);
                case (uint)MafConstants.EditorCommands.Up:
                    return QueryUpKeyStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                case (uint)MafConstants.EditorCommands.Down:
                    return QueryDownKeyStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                case (uint)MafConstants.EditorCommands.Home:
                    return QueryDocumentStartStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                case (uint)MafConstants.EditorCommands.End:
                    return QueryDocumentEndStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                case (uint)MafConstants.EditorCommands.BeginOfLine:
                    return QueryLineStartStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                case (uint)MafConstants.EditorCommands.BeginOfLineExt:
                    return QueryLineStartExtendStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                case (uint)MafConstants.EditorCommands.EndOfLine:
                    return QueryLineEndStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                case (uint)MafConstants.EditorCommands.EndOfLineExt:
                    return QueryLineEndExtendStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                case (uint)MafConstants.EditorCommands.PageUp:
                    return QueryPageUpKeyStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                case (uint)MafConstants.EditorCommands.PageDown:
                    return QueryPageDownKeyStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                case (uint)MafConstants.EditorCommands.SelectAll:
                    return QuerySelectAllStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                //case VSConstants.VSStd2KCmdID.CUT:
                //    return QueryCutStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                case (uint)MafConstants.EditorCommands.Copy:
                    return QueryCopyStatus(pguidCmdGroup, commandCount, prgCmds, commandText);
                //case VSConstants.VSStd2KCmdID.PASTE:
                //    return QueryPasteStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                //case VSConstants.VSStd2KCmdID.OPENLINEABOVE:
                //    return QueryOpenLineAboveStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                //case VSConstants.VSStd2KCmdID.OPENLINEBELOW:
                //    return QueryOpenLineBelowStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                //case VSConstants.VSStd2KCmdID.UNDO:
                //    return QueryUndoStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                //case VSConstants.VSStd2KCmdID.REDO:
                //    return QueryRedoStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                //case VSConstants.VSStd2KCmdID.DELETEWORDRIGHT:
                //    return QueryWordDeleteToEndStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                //case VSConstants.VSStd2KCmdID.DELETEWORDLEFT:
                //    return QueryWordDeleteToStartStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                //case VSConstants.VSStd2KCmdID.COMMENTBLOCK:
                //case VSConstants.VSStd2KCmdID.COMMENT_BLOCK:
                //    return QueryCommentSelectionStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                //case VSConstants.VSStd2KCmdID.UNCOMMENTBLOCK:
                //case VSConstants.VSStd2KCmdID.UNCOMMENT_BLOCK:
                //    return QueryUncommentSelectionStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                //case VSConstants.VSStd2KCmdID.CANCEL:
                //    return QueryEscapeKeyStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                //case VSConstants.VSStd2KCmdID.PARAMINFO:
                //    return QueryInvokeSignatureHelpStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                //case VSConstants.VSStd2KCmdID.COMPLETEWORD:
                //    return QueryCommitUniqueCompletionListItemStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                //case VSConstants.VSStd2KCmdID.SHOWMEMBERLIST:
                //    return QueryInvokeCompletionListStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                //case VSConstants.VSStd2KCmdID.FORMATSELECTION:
                //    return QueryFormatSelectionStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                //case VSConstants.VSStd2KCmdID.QUICKINFO:
                //    return QueryInvokeQuickInfoStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                //case VSConstants.VSStd2KCmdID.FORMATDOCUMENT:
                //    return QueryFormatDocumentStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                //case VSConstants.VSStd2KCmdID.OUTLN_START_AUTOHIDING:
                //    return QueryStartAutomaticOutliningStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                //case VSConstants.VSStd2KCmdID.INSERTSNIPPET:
                //    return QueryInsertSnippetStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                //case VSConstants.VSStd2KCmdID.ECMD_NEXTMETHOD:
                //    return QueryGoToNextMemberStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                //case VSConstants.VSStd2KCmdID.ECMD_PREVMETHOD:
                //    return QueryGoToPreviousMemberStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                //case VSConstants.VSStd2KCmdID.ECMD_INSERTCOMMENT:
                //    return QueryInsertCommentStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                //case VSConstants.VSStd2KCmdID.EXPANDSELECTION:
                //    return QueryExpandSelectionStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                //case VSConstants.VSStd2KCmdID.COLLAPSESELECTION:
                //    return QueryContractSelectionStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                //case VSConstants.VSStd2KCmdID.RENAME:
                //    return QueryRenameStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                //case VSConstants.VSStd2KCmdID.EXTRACTMETHOD:
                //    return QueryExtractMethodStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                //case VSConstants.VSStd2KCmdID.ENCAPSULATEFIELD:
                //    return QueryEncapsulateFieldStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                //case VSConstants.VSStd2KCmdID.EXTRACTINTERFACE:
                //    return QueryExtractInterfaceStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                //case VSConstants.VSStd2KCmdID.REMOVEPARAMETERS:
                //    return QueryRemoveParametersStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                //case VSConstants.VSStd2KCmdID.REORDERPARAMETERS:
                //    return QueryReorderParametersStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                //case VSConstants.VSStd2KCmdID.SURROUNDWITH:
                //    return QuerySurroundWithStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                //case VSConstants.VSStd2KCmdID.ViewCallHierarchy:
                //    return QueryViewCallHierarchyStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                //case VSConstants.VSStd2KCmdID.ToggleConsumeFirstCompletionMode:
                //    return QueryToggleCompletionModeStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                //case VSConstants.VSStd2KCmdID.PROJSETTINGS | VSConstants.VSStd2KCmdID.CallBrowser3ShowFullNames:
                //    return QueryNavigateToNextHighlightedReferenceStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                //case VSConstants.VSStd2KCmdID.LINKONLY | VSConstants.VSStd2KCmdID.CallBrowser3ShowFullNames:
                //    return QueryNavigateToPreviousHighlightedReferenceStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                default:
                    return 1;
            }
        }

        private int QueryCustomCommandStatus(Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds,
            IntPtr commandText)
        {
            var key = new ValueTuple<Guid, uint>(pguidCmdGroup, prgCmds[0].cmdID);
            if (_commandBindings.TryGetValue(key, out var typeName))
            {
                var type = Type.GetType(typeName, false);
                if (type != null)
                    return (int) GetType()
                        .GetMethod(nameof(GetCommandStateHelper), BindingFlags.Instance | BindingFlags.NonPublic)
                        .MakeGenericMethod(type).Invoke(this, new object[5]
                        {
                            type,
                            pguidCmdGroup,
                            commandCount,
                            prgCmds,
                            commandText
                        });
            }

            return 1;
        }

        private int GetCommandStateHelper<T>(Type commandArgsType, Guid pguidCmdGroup, uint commandCount,
            Olecmd[] prgCmds, IntPtr commandText) where T : EditorCommandArgs
        {
            return GetCommandState((view, buffer) => TryCreateCustomCommandArgs<T>(commandArgsType, view, buffer),
                pguidCmdGroup, commandCount, prgCmds, commandText);
        }

        private int GetCommandState<T>(Func<ITextView, ITextBuffer, T> argsFactory, Guid pguidCmdGroup,
            uint commandCount, Olecmd[] prgCmds, IntPtr commandText) where T : EditorCommandArgs
        {
            var guidCmdGroup = pguidCmdGroup;
            var commandState = _commandHandlerService.GetCommandState(argsFactory, () =>
            {
                NextCommandTarget.QueryStatus(guidCmdGroup, commandCount, prgCmds, commandText);
                return new CommandState(((int) prgCmds[0].cmdf & 2) == 2, ((int) prgCmds[0].cmdf & 4) == 4,
                    Utilities.Utilities.GetCmdText(commandText));
            });
            if (!commandState.IsAvailable)
                return 1;
            var olecmdf1 = commandState.IsAvailable ? Olecmdf.Enabled : Olecmdf.Invisible;
            var olecmdf2 = commandState.IsChecked ? Olecmdf.Latched : Olecmdf.Ninched;
            prgCmds[0].cmdf = olecmdf1 | olecmdf2 | Olecmdf.Supported;
            if (!string.IsNullOrEmpty(commandState.DisplayText) &&
                Utilities.Utilities.GetCmdText(commandText) != commandState.DisplayText)
                Utilities.Utilities.SetCmdText(commandText, commandState.DisplayText);
            return 0;
        }
    }

    internal partial class CommandHandlerServiceAdapter
    {
    }
}