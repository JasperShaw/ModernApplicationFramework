using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using Caliburn.Micro;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Ui.Commanding;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Editor.Commanding;
using ModernApplicationFramework.Text.Ui.Editor.Commanding.Commands;
using Action = System.Action;

namespace ModernApplicationFramework.Editor.Implementation
{
    internal class CommandHandlerServiceAdapter : ICommandHandlerServiceAdapter
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

        public int QueryStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        {
            var num = !(pguidCmdGroup == MafConstants.EditorCommandGroup)
                ? QueryCustomCommandStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText)
                : QueryEditorCommandGroup(ref pguidCmdGroup, commandCount, prgCmds, commandText);
            if (num == 0)
                return num;
            if (NextCommandTarget == null)
                return -2147467259;
            return NextCommandTarget.QueryStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
        }

        public int Exec(ref Guid pguidCmdGroup, uint nCmdId, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            if (nCmdexecopt == 65539U)
            {
                var nextCommandTarget = NextCommandTarget;
                if (nextCommandTarget == null)
                    return -2147467259;
                return nextCommandTarget.Exec(ref pguidCmdGroup, nCmdId, nCmdexecopt, pvaIn, pvaOut);
            }

            var nextTargetResult = new int?();
            var guidCmdGroup = pguidCmdGroup;
            var num2 = !(guidCmdGroup == MafConstants.EditorCommandGroup)
                ? ExecuteCustomCommand(ref guidCmdGroup, nCmdId, pvaIn, pvaOut, () =>
                {
                    var nextCommandTarget = NextCommandTarget;
                    nextTargetResult =
                        nextCommandTarget?.Exec(ref guidCmdGroup, nCmdId, nCmdexecopt, pvaIn, pvaOut) ??
                        -2147467259;
                })
                : ExecuteVisualStudio2000(ref guidCmdGroup, nCmdId, pvaIn, pvaOut, () =>
                {
                    var nextCommandTarget = NextCommandTarget;
                    nextTargetResult = nextCommandTarget?.Exec(ref guidCmdGroup, nCmdId, nCmdexecopt, pvaIn, pvaOut) ??
                                       -2147467259;
                });
            if (num2 == -2147221244)
            {
                var nextCommandTarget = NextCommandTarget;
                return nextCommandTarget?.Exec(ref guidCmdGroup, nCmdId, nCmdexecopt, pvaIn, pvaOut) ?? -2147467259;
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

        private int ExecuteVisualStudio2000(ref Guid pguidCmdGroup, uint commandId, IntPtr pvaIn, IntPtr pvaOut,
            Action next)
        {
            switch (commandId)
            {
                case (uint) MafConstants.EditorCommands.TypeChar:
                    var forNativeVariant = (char)(ushort)Marshal.GetObjectForNativeVariant(pvaIn);
                    ExecuteTypeCharCommand(next, forNativeVariant);
                    return 0;
                case (uint) MafConstants.EditorCommands.Backspace:
                    ExecuteBackspaceKeyCommand(next);
                    return 0;
                case (uint )MafConstants.EditorCommands.Return:
                    ExecuteReturnKeyCommand(next);
                    return 0;
                //case VSConstants.VSStd2KCmdID.TAB:
                //    ExecuteTabKeyCommand(next);
                //    return 0;
                //case VSConstants.VSStd2KCmdID.BACKTAB:
                //    ExecuteBackTabKeyCommand(next);
                //    return 0;
                //case VSConstants.VSStd2KCmdID.DELETE:
                //    ExecuteDeleteKeyCommand(next);
                //    return 0;
                case (uint) MafConstants.EditorCommands.Left:
                    ExecuteLeftKeyCommand(next);
                    return 0;
                //case VSConstants.VSStd2KCmdID.RIGHT:
                //    ExecuteRightKeyCommand(next);
                //    return 0;
                //case VSConstants.VSStd2KCmdID.UP:
                //    ExecuteUpKeyCommand(next);
                //    return 0;
                //case VSConstants.VSStd2KCmdID.DOWN:
                //    ExecuteDownKeyCommand(next);
                //    return 0;
                //case VSConstants.VSStd2KCmdID.HOME:
                //    ExecuteDocumentStartCommand(next);
                //    return 0;
                //case VSConstants.VSStd2KCmdID.END:
                //    ExecuteDocumentEndCommand(next);
                //    return 0;
                //case VSConstants.VSStd2KCmdID.BOL:
                //    ExecuteLineStartCommand(next);
                //    return 0;
                //case VSConstants.VSStd2KCmdID.BOL_EXT:
                //    ExecuteLineStartExtendCommand(next);
                //    return 0;
                //case VSConstants.VSStd2KCmdID.EOL:
                //    ExecuteLineEndCommand(next);
                //    return 0;
                //case VSConstants.VSStd2KCmdID.EOL_EXT:
                //    ExecuteLineEndExtendCommand(next);
                //    return 0;
                //case VSConstants.VSStd2KCmdID.PAGEUP:
                //    ExecutePageUpKeyCommand(next);
                //    return 0;
                //case VSConstants.VSStd2KCmdID.PAGEDN:
                //    ExecutePageDownKeyCommand(next);
                //    return 0;
                //case VSConstants.VSStd2KCmdID.SELECTALL:
                //    ExecuteSelectAllCommand(next);
                //    return 0;
                //case VSConstants.VSStd2KCmdID.CUT:
                //    ExecuteCutCommand(next);
                //    return 0;
                //case VSConstants.VSStd2KCmdID.COPY:
                //    ExecuteCopyCommand(next);
                //    return 0;
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

        private int ExecuteCustomCommand(ref Guid pguidCmdGroup, uint commandId, IntPtr pvaIn, IntPtr pvaOut,
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

        private int QueryEditorCommandGroup(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds,
            IntPtr commandText)
        {
            switch (prgCmds[0].cmdID)
            {
                //case VSConstants.VSStd2KCmdID.BACKSPACE:
                //    return QueryBackspaceKeyStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                //case VSConstants.VSStd2KCmdID.RETURN:
                //    return QueryReturnKeyStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                //case VSConstants.VSStd2KCmdID.TAB:
                //    return QueryTabKeyStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                //case VSConstants.VSStd2QueryLeftKeyStatusKCmdID.BACKTAB:
                //    return QueryBackTabKeyStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                //case VSConstants.VSStd2KCmdID.DELETE:
                //    return QueryDeleteKeyStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                case (uint) MafConstants.EditorCommands.Left:
                    return QueryLeftKeyStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                //case VSConstants.VSStd2KCmdID.RIGHT:
                //    return QueryRightKeyStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                //case VSConstants.VSStd2KCmdID.UP:
                //    return QueryUpKeyStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                //case VSConstants.VSStd2KCmdID.DOWN:
                //    return QueryDownKeyStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                //case VSConstants.VSStd2KCmdID.HOME:
                //    return QueryDocumentStartStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                //case VSConstants.VSStd2KCmdID.END:
                //    return QueryDocumentEndStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                //case VSConstants.VSStd2KCmdID.BOL:
                //    return QueryLineStartStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                //case VSConstants.VSStd2KCmdID.BOL_EXT:
                //    return QueryLineStartExtendStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                //case VSConstants.VSStd2KCmdID.EOL:
                //    return QueryLineEndStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                //case VSConstants.VSStd2KCmdID.EOL_EXT:
                //    return QueryLineEndExtendStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                //case VSConstants.VSStd2KCmdID.PAGEUP:
                //    return QueryPageUpKeyStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                //case VSConstants.VSStd2KCmdID.PAGEDN:
                //    return QueryPageDownKeyStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                //case VSConstants.VSStd2KCmdID.SELECTALL:
                //    return QuerySelectAllStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                //case VSConstants.VSStd2KCmdID.CUT:
                //    return QueryCutStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
                //case VSConstants.VSStd2KCmdID.COPY:
                //    return QueryCopyStatus(ref pguidCmdGroup, commandCount, prgCmds, commandText);
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

        private int QueryCustomCommandStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds,
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

        private int GetCommandStateHelper<T>(Type commandArgsType, ref Guid pguidCmdGroup, uint commandCount,
            Olecmd[] prgCmds, IntPtr commandText) where T : EditorCommandArgs
        {
            return GetCommandState((view, buffer) => TryCreateCustomCommandArgs<T>(commandArgsType, view, buffer),
                ref pguidCmdGroup, commandCount, prgCmds, commandText);
        }

        private int GetCommandState<T>(Func<ITextView, ITextBuffer, T> argsFactory, ref Guid pguidCmdGroup,
            uint commandCount, Olecmd[] prgCmds, IntPtr commandText) where T : EditorCommandArgs
        {
            var guidCmdGroup = pguidCmdGroup;
            var commandState = _commandHandlerService.GetCommandState(argsFactory, () =>
            {
                NextCommandTarget.QueryStatus(ref guidCmdGroup, commandCount, prgCmds, commandText);
                return new CommandState(((int) prgCmds[0].cmdf & 2) == 2, ((int) prgCmds[0].cmdf & 4) == 4,
                    Utilities.Utilities.GetCmdText(commandText));
            });
            if (!commandState.IsAvailable)
                return 1;
            var olecmdf1 = commandState.IsAvailable ? Olecmdf.OlecmdfEnabled : Olecmdf.OlecmdfInvisible;
            var olecmdf2 = commandState.IsChecked ? Olecmdf.OlecmdfLatched : Olecmdf.OlecmdfNinched;
            prgCmds[0].cmdf = (uint) (olecmdf1 | olecmdf2 | Olecmdf.OlecmdfSupported);
            if (!string.IsNullOrEmpty(commandState.DisplayText) &&
                Utilities.Utilities.GetCmdText(commandText) != commandState.DisplayText)
                Utilities.Utilities.SetCmdText(commandText, commandState.DisplayText);
            return 0;
        }


        private int QueryLeftKeyStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        {
            return GetCommandState((view, buffer) => new LeftKeyCommandArgs(view, buffer), ref pguidCmdGroup,
                commandCount, prgCmds, commandText);
        }


        private void ExecuteTypeCharCommand(Action next, char typedChar)
        {
            _commandHandlerService.Execute((textView, subjectBuffer) => new TypeCharCommandArgs(TextView, subjectBuffer, typedChar), next);
        }

        private void ExecuteBackspaceKeyCommand(Action next)
        {
            var commandHandlerService = _commandHandlerService;
            var nextCommandHandler = next;
            commandHandlerService.Execute((view, buffer) => new BackspaceKeyCommandArgs(view, buffer), nextCommandHandler);
        }

        private void ExecuteReturnKeyCommand(Action next)
        {
            IEditorCommandHandlerService commandHandlerService = _commandHandlerService;
            Action nextCommandHandler = next;
            commandHandlerService.Execute((view, buffer) => new ReturnKeyCommandArgs(view, buffer), nextCommandHandler);
        }

        private void ExecuteLeftKeyCommand(Action next)
        {
            var commandHandlerService = _commandHandlerService;
            var nextCommandHandler = next;
            commandHandlerService.Execute((view, buffer) => new LeftKeyCommandArgs(view, buffer), nextCommandHandler);
        }

    }

    public sealed class ReturnKeyCommandArgs : EditorCommandArgs
    {
        public ReturnKeyCommandArgs(ITextView textView, ITextBuffer subjectBuffer)
            : base(textView, subjectBuffer)
        {
        }
    }
}