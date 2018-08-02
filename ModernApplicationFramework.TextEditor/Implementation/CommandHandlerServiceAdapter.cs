using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Reflection;
using Caliburn.Micro;
using ModernApplicationFramework.TextEditor.Utilities;
using ModernApplicationFramework.Utilities.Interfaces;
using Action = System.Action;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    internal class CommandHandlerServiceAdapter : ICommandHandlerServiceAdapter
    {
        //public static readonly Guid InteractiveCommandSetId = new Guid("00B8868B-F9F5-4970-A048-410B05508506");
        private readonly IEditorCommandHandlerService _commandHandlerService;
        private static Dictionary<ValueTuple<Guid, uint>, string> _commandBindings;
        public const int ExecuteInInteractiveWindow = 268;

        public CommandHandlerServiceAdapter(ITextView textView, ICommandTarget nextCommandTarget)
            : this(textView, null, nextCommandTarget)
        {
        }

        public CommandHandlerServiceAdapter(ITextView textView, ITextBuffer subjectBuffer, ICommandTarget nextCommandTarget)
        {
            var textView1 = textView;
            TextView = textView1 ?? throw new ArgumentNullException(nameof(textView));
            NextCommandTarget = nextCommandTarget;
            _commandHandlerService = IoC.Get<IEditorCommandHandlerServiceFactory>().GetService(TextView, subjectBuffer);
            InitializeCommandBindings();
        }

        private void InitializeCommandBindings()
        {
            var list = IoC.GetAll<Lazy<CommandBindingDefinition, ICommandBindingMetadata>>();



            if (_commandBindings != null)
                return;
            _commandBindings = new Dictionary<ValueTuple<Guid, uint>, string>();
            foreach (var export in list)
            {
                var metadata = export.Metadata;
                if (metadata.CommandSet != null && metadata.CommandArgsType != null && (metadata.CommandId != null && metadata.CommandId.Length == metadata.CommandSet.Length) && metadata.CommandSet.Length == metadata.CommandArgsType.Length)
                {
                    for (var index = 0; index < export.Metadata.CommandSet.Length; ++index)
                    {
                        if (Guid.TryParse(metadata.CommandSet[index], out var result))
                            _commandBindings[new ValueTuple<Guid, uint>(result, metadata.CommandId[index])] = metadata.CommandArgsType[index];
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
            var num1 = -2147221244;
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
                var num3 = 1;
                if ((nullable1.GetValueOrDefault() == num3 ? (nullable1.HasValue ? 1 : 0) : 0) == 0)
                {
                    var nullable2 = nextTargetResult;
                    var num4 = -2147467259;
                    if ((nullable2.GetValueOrDefault() == num4 ? (nullable2.HasValue ? 1 : 0) : 0) == 0)
                        goto label_10;
                }
                return nextTargetResult.Value;
            }
            label_10:
            return num2;
        }

        private int ExecuteVisualStudio2000(ref Guid pguidCmdGroup, uint commandId, IntPtr pvaIn, IntPtr pvaOut, Action next)
        {
            switch (commandId)
            {
                //case VSConstants.VSStd2KCmdID.TYPECHAR:
                //    var forNativeVariant = (char)(ushort)Marshal.GetObjectForNativeVariant(pvaIn);
                //    ExecuteTypeCharCommand(next, forNativeVariant);
                //    return 0;
                //case VSConstants.VSStd2KCmdID.BACKSPACE:
                //    ExecuteBackspaceKeyCommand(next);
                //    return 0;
                //case VSConstants.VSStd2KCmdID.RETURN:
                //    ExecuteReturnKeyCommand(next);
                //    return 0;
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

        private int ExecuteCustomCommand(ref Guid pguidCmdGroup, uint commandId, IntPtr pvaIn, IntPtr pvaOut, Action next)
        {
            var key = new ValueTuple<Guid, uint>(pguidCmdGroup, commandId);
            if (_commandBindings.TryGetValue(key, out var typeName))
            {
                var type = Type.GetType(typeName, false);
                if (type != null)
                {
                    GetType().GetMethod(nameof(ExecuteCustomCommandHelper), BindingFlags.Instance | BindingFlags.NonPublic).MakeGenericMethod(type).Invoke(this, new object[2]
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

        private void ExecuteCustomCommandHelper<T>(Type commandArgsType, Action nextCommandHandler) where T : EditorCommandArgs
        {
            _commandHandlerService.Execute((view, buffer) => TryCreateCustomCommandArgs<T>(commandArgsType, view, buffer), nextCommandHandler);
        }

        private int QueryEditorCommandGroup(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
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

        private int QueryCustomCommandStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        {
            var key = new ValueTuple<Guid, uint>(pguidCmdGroup, prgCmds[0].cmdID);
            if (_commandBindings.TryGetValue(key, out var typeName))
            {
                var type = Type.GetType(typeName, false);
                if (type != null)
                    return (int)GetType().GetMethod(nameof(GetCommandStateHelper), BindingFlags.Instance | BindingFlags.NonPublic).MakeGenericMethod(type).Invoke(this, new object[5]
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

        private int GetCommandStateHelper<T>(Type commandArgsType, ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText) where T : EditorCommandArgs
        {
            return GetCommandState((view, buffer) => TryCreateCustomCommandArgs<T>(commandArgsType, view, buffer), ref pguidCmdGroup, commandCount, prgCmds, commandText);
        }

        //private void ExecuteAutomaticLineEnderCommand(Action next)
        //{
        //    IEditorCommandHandlerService commandHandlerService = _commandHandlerService;
        //    var nextCommandHandler = next;
        //    commandHandlerService.Execute<AutomaticLineEnderCommandArgs>((Func<ITextView, ITextBuffer, AutomaticLineEnderCommandArgs>)((textView, subjectBuffer) => new AutomaticLineEnderCommandArgs(textView, subjectBuffer)), nextCommandHandler);
        //}

        //private void ExecuteBackspaceKeyCommand(Action next)
        //{
        //    IEditorCommandHandlerService commandHandlerService = _commandHandlerService;
        //    var nextCommandHandler = next;
        //    commandHandlerService.Execute<BackspaceKeyCommandArgs>((Func<ITextView, ITextBuffer, BackspaceKeyCommandArgs>)((view, buffer) => new BackspaceKeyCommandArgs(view, buffer)), nextCommandHandler);
        //}

        //private void ExecuteBackTabKeyCommand(Action next)
        //{
        //    IEditorCommandHandlerService commandHandlerService = _commandHandlerService;
        //    var nextCommandHandler = next;
        //    commandHandlerService.Execute<BackTabKeyCommandArgs>((Func<ITextView, ITextBuffer, BackTabKeyCommandArgs>)((view, buffer) => new BackTabKeyCommandArgs(view, buffer)), nextCommandHandler);
        //}

        //private void ExecuteCommentSelectionCommand(Action next)
        //{
        //    IEditorCommandHandlerService commandHandlerService = _commandHandlerService;
        //    var nextCommandHandler = next;
        //    commandHandlerService.Execute<CommentSelectionCommandArgs>((Func<ITextView, ITextBuffer, CommentSelectionCommandArgs>)((view, buffer) => new CommentSelectionCommandArgs(view, buffer)), nextCommandHandler);
        //}

        //private void ExecuteCommitUniqueCompletionListItemCommand(Action next)
        //{
        //    IEditorCommandHandlerService commandHandlerService = _commandHandlerService;
        //    var nextCommandHandler = next;
        //    commandHandlerService.Execute<CommitUniqueCompletionListItemCommandArgs>((Func<ITextView, ITextBuffer, CommitUniqueCompletionListItemCommandArgs>)((view, buffer) => new CommitUniqueCompletionListItemCommandArgs(view, buffer)), nextCommandHandler);
        //}

        //private void ExecuteCopyCommand(Action next)
        //{
        //    IEditorCommandHandlerService commandHandlerService = _commandHandlerService;
        //    var nextCommandHandler = next;
        //    commandHandlerService.Execute<CopyCommandArgs>((Func<ITextView, ITextBuffer, CopyCommandArgs>)((view, buffer) => new CopyCommandArgs(view, buffer)), nextCommandHandler);
        //}

        //private void ExecuteCutCommand(Action next)
        //{
        //    IEditorCommandHandlerService commandHandlerService = _commandHandlerService;
        //    var nextCommandHandler = next;
        //    commandHandlerService.Execute<CutCommandArgs>((Func<ITextView, ITextBuffer, CutCommandArgs>)((view, buffer) => new CutCommandArgs(view, buffer)), nextCommandHandler);
        //}

        //private void ExecuteDeleteKeyCommand(Action next)
        //{
        //    IEditorCommandHandlerService commandHandlerService = _commandHandlerService;
        //    var nextCommandHandler = next;
        //    commandHandlerService.Execute<DeleteKeyCommandArgs>((Func<ITextView, ITextBuffer, DeleteKeyCommandArgs>)((view, buffer) => new DeleteKeyCommandArgs(view, buffer)), nextCommandHandler);
        //}

        //private void ExecuteDocumentEndCommand(Action next)
        //{
        //    IEditorCommandHandlerService commandHandlerService = _commandHandlerService;
        //    var nextCommandHandler = next;
        //    commandHandlerService.Execute<DocumentEndCommandArgs>((Func<ITextView, ITextBuffer, DocumentEndCommandArgs>)((view, buffer) => new DocumentEndCommandArgs(view, buffer)), nextCommandHandler);
        //}

        //private void ExecuteDocumentStartCommand(Action next)
        //{
        //    IEditorCommandHandlerService commandHandlerService = _commandHandlerService;
        //    var nextCommandHandler = next;
        //    commandHandlerService.Execute<DocumentStartCommandArgs>((Func<ITextView, ITextBuffer, DocumentStartCommandArgs>)((view, buffer) => new DocumentStartCommandArgs(view, buffer)), nextCommandHandler);
        //}

        //private void ExecuteDownKeyCommand(Action next)
        //{
        //    IEditorCommandHandlerService commandHandlerService = _commandHandlerService;
        //    var nextCommandHandler = next;
        //    commandHandlerService.Execute<DownKeyCommandArgs>((Func<ITextView, ITextBuffer, DownKeyCommandArgs>)((view, buffer) => new DownKeyCommandArgs(view, buffer)), nextCommandHandler);
        //}

        //private void ExecuteEncapsulateFieldCommand(Action next)
        //{
        //    IEditorCommandHandlerService commandHandlerService = _commandHandlerService;
        //    var nextCommandHandler = next;
        //    commandHandlerService.Execute<EncapsulateFieldCommandArgs>((Func<ITextView, ITextBuffer, EncapsulateFieldCommandArgs>)((view, buffer) => new EncapsulateFieldCommandArgs(view, buffer)), nextCommandHandler);
        //}

        //private void ExecuteEscapeKeyCommand(Action next)
        //{
        //    IEditorCommandHandlerService commandHandlerService = _commandHandlerService;
        //    var nextCommandHandler = next;
        //    commandHandlerService.Execute<EscapeKeyCommandArgs>((Func<ITextView, ITextBuffer, EscapeKeyCommandArgs>)((view, buffer) => new EscapeKeyCommandArgs(view, buffer)), nextCommandHandler);
        //}

        //private void ExecuteExecuteInInteractiveCommand(Action next)
        //{
        //    IEditorCommandHandlerService commandHandlerService = _commandHandlerService;
        //    var nextCommandHandler = next;
        //    commandHandlerService.Execute<ExecuteInInteractiveCommandArgs>((Func<ITextView, ITextBuffer, ExecuteInInteractiveCommandArgs>)((view, buffer) => new ExecuteInInteractiveCommandArgs(view, buffer)), nextCommandHandler);
        //}

        //private void ExecuteExtractInterfaceCommand(Action next)
        //{
        //    IEditorCommandHandlerService commandHandlerService = _commandHandlerService;
        //    var nextCommandHandler = next;
        //    commandHandlerService.Execute<ExtractInterfaceCommandArgs>((Func<ITextView, ITextBuffer, ExtractInterfaceCommandArgs>)((view, buffer) => new ExtractInterfaceCommandArgs(view, buffer)), nextCommandHandler);
        //}

        //private void ExecuteExtractMethodCommand(Action next)
        //{
        //    IEditorCommandHandlerService commandHandlerService = _commandHandlerService;
        //    var nextCommandHandler = next;
        //    commandHandlerService.Execute<ExtractMethodCommandArgs>((Func<ITextView, ITextBuffer, ExtractMethodCommandArgs>)((view, buffer) => new ExtractMethodCommandArgs(view, buffer)), nextCommandHandler);
        //}

        //private void ExecuteFindReferencesCommand(Action next)
        //{
        //    IEditorCommandHandlerService commandHandlerService = _commandHandlerService;
        //    var nextCommandHandler = next;
        //    commandHandlerService.Execute<FindReferencesCommandArgs>((Func<ITextView, ITextBuffer, FindReferencesCommandArgs>)((view, buffer) => new FindReferencesCommandArgs(view, buffer)), nextCommandHandler);
        //}

        //private void ExecuteFormatDocumentCommand(Action next)
        //{
        //    IEditorCommandHandlerService commandHandlerService = _commandHandlerService;
        //    var nextCommandHandler = next;
        //    commandHandlerService.Execute<FormatDocumentCommandArgs>((Func<ITextView, ITextBuffer, FormatDocumentCommandArgs>)((view, buffer) => new FormatDocumentCommandArgs(view, buffer)), nextCommandHandler);
        //}

        //private void ExecuteFormatSelectionCommand(Action next)
        //{
        //    IEditorCommandHandlerService commandHandlerService = _commandHandlerService;
        //    var nextCommandHandler = next;
        //    commandHandlerService.Execute<FormatSelectionCommandArgs>((Func<ITextView, ITextBuffer, FormatSelectionCommandArgs>)((view, buffer) => new FormatSelectionCommandArgs(view, buffer)), nextCommandHandler);
        //}

        //private void ExecuteGoToNextMemberCommand(Action next)
        //{
        //    IEditorCommandHandlerService commandHandlerService = _commandHandlerService;
        //    var nextCommandHandler = next;
        //    commandHandlerService.Execute<GoToNextMemberCommandArgs>((Func<ITextView, ITextBuffer, GoToNextMemberCommandArgs>)((view, buffer) => new GoToNextMemberCommandArgs(view, buffer)), nextCommandHandler);
        //}

        //private void ExecuteGoToPreviousMemberCommand(Action next)
        //{
        //    IEditorCommandHandlerService commandHandlerService = _commandHandlerService;
        //    var nextCommandHandler = next;
        //    commandHandlerService.Execute<GoToPreviousMemberCommandArgs>((Func<ITextView, ITextBuffer, GoToPreviousMemberCommandArgs>)((view, buffer) => new GoToPreviousMemberCommandArgs(view, buffer)), nextCommandHandler);
        //}

        //private void ExecuteGoToDefinitionCommand(Action next)
        //{
        //    IEditorCommandHandlerService commandHandlerService = _commandHandlerService;
        //    var nextCommandHandler = next;
        //    commandHandlerService.Execute<GoToDefinitionCommandArgs>((Func<ITextView, ITextBuffer, GoToDefinitionCommandArgs>)((view, buffer) => new GoToDefinitionCommandArgs(view, buffer)), nextCommandHandler);
        //}

        //private void ExecuteInsertCommentCommand(Action next)
        //{
        //    IEditorCommandHandlerService commandHandlerService = _commandHandlerService;
        //    var nextCommandHandler = next;
        //    commandHandlerService.Execute<InsertCommentCommandArgs>((Func<ITextView, ITextBuffer, InsertCommentCommandArgs>)((view, buffer) => new InsertCommentCommandArgs(view, buffer)), nextCommandHandler);
        //}

        //private void ExecuteInsertSnippetCommand(Action next)
        //{
        //    IEditorCommandHandlerService commandHandlerService = _commandHandlerService;
        //    var nextCommandHandler = next;
        //    commandHandlerService.Execute<InsertSnippetCommandArgs>((Func<ITextView, ITextBuffer, InsertSnippetCommandArgs>)((view, buffer) => new InsertSnippetCommandArgs(view, buffer)), nextCommandHandler);
        //}

        //private void ExecuteInvokeCompletionListCommand(Action next)
        //{
        //    IEditorCommandHandlerService commandHandlerService = _commandHandlerService;
        //    var nextCommandHandler = next;
        //    commandHandlerService.Execute<InvokeCompletionListCommandArgs>((Func<ITextView, ITextBuffer, InvokeCompletionListCommandArgs>)((view, buffer) => new InvokeCompletionListCommandArgs(view, buffer)), nextCommandHandler);
        //}

        //private void ExecuteInvokeQuickInfoCommand(Action next)
        //{
        //    IEditorCommandHandlerService commandHandlerService = _commandHandlerService;
        //    var nextCommandHandler = next;
        //    commandHandlerService.Execute<InvokeQuickInfoCommandArgs>((Func<ITextView, ITextBuffer, InvokeQuickInfoCommandArgs>)((view, buffer) => new InvokeQuickInfoCommandArgs(view, buffer)), nextCommandHandler);
        //}

        //private void ExecuteInvokeSignatureHelpCommand(Action next)
        //{
        //    IEditorCommandHandlerService commandHandlerService = _commandHandlerService;
        //    var nextCommandHandler = next;
        //    commandHandlerService.Execute<InvokeSignatureHelpCommandArgs>((Func<ITextView, ITextBuffer, InvokeSignatureHelpCommandArgs>)((view, buffer) => new InvokeSignatureHelpCommandArgs(view, buffer)), nextCommandHandler);
        //}

        //private void ExecuteLineEndCommand(Action next)
        //{
        //    IEditorCommandHandlerService commandHandlerService = _commandHandlerService;
        //    var nextCommandHandler = next;
        //    commandHandlerService.Execute<LineEndCommandArgs>((Func<ITextView, ITextBuffer, LineEndCommandArgs>)((view, buffer) => new LineEndCommandArgs(view, buffer)), nextCommandHandler);
        //}

        //private void ExecuteExpandSelectionCommand(Action next)
        //{
        //    IEditorCommandHandlerService commandHandlerService = _commandHandlerService;
        //    var nextCommandHandler = next;
        //    commandHandlerService.Execute<ExpandSelectionCommandArgs>((Func<ITextView, ITextBuffer, ExpandSelectionCommandArgs>)((view, buffer) => new ExpandSelectionCommandArgs(view, buffer)), nextCommandHandler);
        //}

        //private void ExecuteContractSelectionCommand(Action next)
        //{
        //    IEditorCommandHandlerService commandHandlerService = _commandHandlerService;
        //    var nextCommandHandler = next;
        //    commandHandlerService.Execute<ContractSelectionCommandArgs>((Func<ITextView, ITextBuffer, ContractSelectionCommandArgs>)((view, buffer) => new ContractSelectionCommandArgs(view, buffer)), nextCommandHandler);
        //}

        //private void ExecuteLineEndExtendCommand(Action next)
        //{
        //    IEditorCommandHandlerService commandHandlerService = _commandHandlerService;
        //    var nextCommandHandler = next;
        //    commandHandlerService.Execute<LineEndExtendCommandArgs>((Func<ITextView, ITextBuffer, LineEndExtendCommandArgs>)((view, buffer) => new LineEndExtendCommandArgs(view, buffer)), nextCommandHandler);
        //}

        //private void ExecuteLineStartCommand(Action next)
        //{
        //    IEditorCommandHandlerService commandHandlerService = _commandHandlerService;
        //    var nextCommandHandler = next;
        //    commandHandlerService.Execute<LineStartCommandArgs>((Func<ITextView, ITextBuffer, LineStartCommandArgs>)((view, buffer) => new LineStartCommandArgs(view, buffer)), nextCommandHandler);
        //}

        //private void ExecuteLineStartExtendCommand(Action next)
        //{
        //    IEditorCommandHandlerService commandHandlerService = _commandHandlerService;
        //    var nextCommandHandler = next;
        //    commandHandlerService.Execute<LineStartExtendCommandArgs>((Func<ITextView, ITextBuffer, LineStartExtendCommandArgs>)((view, buffer) => new LineStartExtendCommandArgs(view, buffer)), nextCommandHandler);
        //}

        //private void ExecuteMoveSelectedLinesDownCommand(Action next)
        //{
        //    IEditorCommandHandlerService commandHandlerService = _commandHandlerService;
        //    var nextCommandHandler = next;
        //    commandHandlerService.Execute<MoveSelectedLinesDownCommandArgs>((Func<ITextView, ITextBuffer, MoveSelectedLinesDownCommandArgs>)((view, buffer) => new MoveSelectedLinesDownCommandArgs(view, buffer)), nextCommandHandler);
        //}

        //private void ExecuteMoveSelectedLinesUpCommand(Action next)
        //{
        //    IEditorCommandHandlerService commandHandlerService = _commandHandlerService;
        //    var nextCommandHandler = next;
        //    commandHandlerService.Execute<MoveSelectedLinesUpCommandArgs>((Func<ITextView, ITextBuffer, MoveSelectedLinesUpCommandArgs>)((view, buffer) => new MoveSelectedLinesUpCommandArgs(view, buffer)), nextCommandHandler);
        //}

        //private void ExecuteNavigateToNextHighlightedReferenceCommand(Action next)
        //{
        //    IEditorCommandHandlerService commandHandlerService = _commandHandlerService;
        //    var nextCommandHandler = next;
        //    commandHandlerService.Execute<NavigateToNextHighlightedReferenceCommandArgs>((Func<ITextView, ITextBuffer, NavigateToNextHighlightedReferenceCommandArgs>)((view, buffer) => new NavigateToNextHighlightedReferenceCommandArgs(view, buffer)), nextCommandHandler);
        //}

        //private void ExecuteNavigateToPreviousHighlightedReferenceCommand(Action next)
        //{
        //    IEditorCommandHandlerService commandHandlerService = _commandHandlerService;
        //    var nextCommandHandler = next;
        //    commandHandlerService.Execute<NavigateToPreviousHighlightedReferenceCommandArgs>((Func<ITextView, ITextBuffer, NavigateToPreviousHighlightedReferenceCommandArgs>)((view, buffer) => new NavigateToPreviousHighlightedReferenceCommandArgs(view, buffer)), nextCommandHandler);
        //}

        //private void ExecuteOpenLineAboveCommand(Action next)
        //{
        //    IEditorCommandHandlerService commandHandlerService = _commandHandlerService;
        //    var nextCommandHandler = next;
        //    commandHandlerService.Execute<OpenLineAboveCommandArgs>((Func<ITextView, ITextBuffer, OpenLineAboveCommandArgs>)((view, buffer) => new OpenLineAboveCommandArgs(view, buffer)), nextCommandHandler);
        //}

        //private void ExecuteOpenLineBelowCommand(Action next)
        //{
        //    IEditorCommandHandlerService commandHandlerService = _commandHandlerService;
        //    var nextCommandHandler = next;
        //    commandHandlerService.Execute<OpenLineBelowCommandArgs>((Func<ITextView, ITextBuffer, OpenLineBelowCommandArgs>)((view, buffer) => new OpenLineBelowCommandArgs(view, buffer)), nextCommandHandler);
        //}

        //private void ExecutePageDownKeyCommand(Action next)
        //{
        //    IEditorCommandHandlerService commandHandlerService = _commandHandlerService;
        //    var nextCommandHandler = next;
        //    commandHandlerService.Execute<PageDownKeyCommandArgs>((Func<ITextView, ITextBuffer, PageDownKeyCommandArgs>)((view, buffer) => new PageDownKeyCommandArgs(view, buffer)), nextCommandHandler);
        //}

        //private void ExecutePageUpKeyCommand(Action next)
        //{
        //    IEditorCommandHandlerService commandHandlerService = _commandHandlerService;
        //    var nextCommandHandler = next;
        //    commandHandlerService.Execute<PageUpKeyCommandArgs>((Func<ITextView, ITextBuffer, PageUpKeyCommandArgs>)((view, buffer) => new PageUpKeyCommandArgs(view, buffer)), nextCommandHandler);
        //}

        //private void ExecutePasteCommand(Action next)
        //{
        //    IEditorCommandHandlerService commandHandlerService = _commandHandlerService;
        //    var nextCommandHandler = next;
        //    commandHandlerService.Execute<PasteCommandArgs>((Func<ITextView, ITextBuffer, PasteCommandArgs>)((view, buffer) => new PasteCommandArgs(view, buffer)), nextCommandHandler);
        //}

        //private void ExecuteRedoCommand(Action next, int count = 1)
        //{
        //    _commandHandlerService.Execute<RedoCommandArgs>((Func<ITextView, ITextBuffer, RedoCommandArgs>)((textView, subjectBuffer) => new RedoCommandArgs(TextView, subjectBuffer, count)), next);
        //}

        //private void ExecuteRemoveParametersCommand(Action next)
        //{
        //    IEditorCommandHandlerService commandHandlerService = _commandHandlerService;
        //    var nextCommandHandler = next;
        //    commandHandlerService.Execute<RemoveParametersCommandArgs>((Func<ITextView, ITextBuffer, RemoveParametersCommandArgs>)((view, buffer) => new RemoveParametersCommandArgs(view, buffer)), nextCommandHandler);
        //}

        //private void ExecuteRenameCommand(Action next)
        //{
        //    IEditorCommandHandlerService commandHandlerService = _commandHandlerService;
        //    var nextCommandHandler = next;
        //    commandHandlerService.Execute<RenameCommandArgs>((Func<ITextView, ITextBuffer, RenameCommandArgs>)((view, buffer) => new RenameCommandArgs(view, buffer)), nextCommandHandler);
        //}

        //private void ExecuteReorderParametersCommand(Action next)
        //{
        //    IEditorCommandHandlerService commandHandlerService = _commandHandlerService;
        //    var nextCommandHandler = next;
        //    commandHandlerService.Execute<ReorderParametersCommandArgs>((Func<ITextView, ITextBuffer, ReorderParametersCommandArgs>)((view, buffer) => new ReorderParametersCommandArgs(view, buffer)), nextCommandHandler);
        //}

        //private void ExecuteReturnKeyCommand(Action next)
        //{
        //    IEditorCommandHandlerService commandHandlerService = _commandHandlerService;
        //    var nextCommandHandler = next;
        //    commandHandlerService.Execute<ReturnKeyCommandArgs>((Func<ITextView, ITextBuffer, ReturnKeyCommandArgs>)((view, buffer) => new ReturnKeyCommandArgs(view, buffer)), nextCommandHandler);
        //}

        //private void ExecuteSelectAllCommand(Action next)
        //{
        //    IEditorCommandHandlerService commandHandlerService = _commandHandlerService;
        //    var nextCommandHandler = next;
        //    commandHandlerService.Execute<SelectAllCommandArgs>((Func<ITextView, ITextBuffer, SelectAllCommandArgs>)((view, buffer) => new SelectAllCommandArgs(view, buffer)), nextCommandHandler);
        //}

        //private void ExecuteStartAutomaticOutliningCommand(Action next)
        //{
        //    IEditorCommandHandlerService commandHandlerService = _commandHandlerService;
        //    var nextCommandHandler = next;
        //    commandHandlerService.Execute<StartAutomaticOutliningCommandArgs>((Func<ITextView, ITextBuffer, StartAutomaticOutliningCommandArgs>)((view, buffer) => new StartAutomaticOutliningCommandArgs(view, buffer)), nextCommandHandler);
        //}

        //private void ExecuteSurroundWithCommand(Action next)
        //{
        //    IEditorCommandHandlerService commandHandlerService = _commandHandlerService;
        //    var nextCommandHandler = next;
        //    commandHandlerService.Execute<SurroundWithCommandArgs>((Func<ITextView, ITextBuffer, SurroundWithCommandArgs>)((view, buffer) => new SurroundWithCommandArgs(view, buffer)), nextCommandHandler);
        //}

        //private void ExecuteSyncClassViewCommand(Action next)
        //{
        //    IEditorCommandHandlerService commandHandlerService = _commandHandlerService;
        //    var nextCommandHandler = next;
        //    commandHandlerService.Execute<SyncClassViewCommandArgs>((Func<ITextView, ITextBuffer, SyncClassViewCommandArgs>)((view, buffer) => new SyncClassViewCommandArgs(view, buffer)), nextCommandHandler);
        //}

        //private void ExecuteTabKeyCommand(Action next)
        //{
        //    IEditorCommandHandlerService commandHandlerService = _commandHandlerService;
        //    var nextCommandHandler = next;
        //    commandHandlerService.Execute<TabKeyCommandArgs>((Func<ITextView, ITextBuffer, TabKeyCommandArgs>)((view, buffer) => new TabKeyCommandArgs(view, buffer)), nextCommandHandler);
        //}

        //private void ExecuteToggleCompletionModeCommand(Action next)
        //{
        //    IEditorCommandHandlerService commandHandlerService = _commandHandlerService;
        //    var nextCommandHandler = next;
        //    commandHandlerService.Execute<ToggleCompletionModeCommandArgs>((Func<ITextView, ITextBuffer, ToggleCompletionModeCommandArgs>)((view, buffer) => new ToggleCompletionModeCommandArgs(view, buffer)), nextCommandHandler);
        //}

        //private void ExecuteTypeCharCommand(Action next, char typedChar)
        //{
        //    _commandHandlerService.Execute<TypeCharCommandArgs>((Func<ITextView, ITextBuffer, TypeCharCommandArgs>)((textView, subjectBuffer) => new TypeCharCommandArgs(TextView, subjectBuffer, typedChar)), next);
        //}

        //private void ExecuteUncommentSelectionCommand(Action next)
        //{
        //    IEditorCommandHandlerService commandHandlerService = _commandHandlerService;
        //    var nextCommandHandler = next;
        //    commandHandlerService.Execute<UncommentSelectionCommandArgs>((Func<ITextView, ITextBuffer, UncommentSelectionCommandArgs>)((view, buffer) => new UncommentSelectionCommandArgs(view, buffer)), nextCommandHandler);
        //}

        //private void ExecuteUndoCommand(Action next, int count = 1)
        //{
        //    _commandHandlerService.Execute<UndoCommandArgs>((Func<ITextView, ITextBuffer, UndoCommandArgs>)((textView, subjectBuffer) => new UndoCommandArgs(TextView, subjectBuffer, count)), next);
        //}

        //private void ExecuteUpKeyCommand(Action next)
        //{
        //    IEditorCommandHandlerService commandHandlerService = _commandHandlerService;
        //    var nextCommandHandler = next;
        //    commandHandlerService.Execute<UpKeyCommandArgs>((Func<ITextView, ITextBuffer, UpKeyCommandArgs>)((view, buffer) => new UpKeyCommandArgs(view, buffer)), nextCommandHandler);
        //}

        //private void ExecuteViewCallHierarchyCommand(Action next)
        //{
        //    IEditorCommandHandlerService commandHandlerService = _commandHandlerService;
        //    var nextCommandHandler = next;
        //    commandHandlerService.Execute<ViewCallHierarchyCommandArgs>((Func<ITextView, ITextBuffer, ViewCallHierarchyCommandArgs>)((view, buffer) => new ViewCallHierarchyCommandArgs(view, buffer)), nextCommandHandler);
        //}

        //private void ExecuteWordDeleteToEndCommand(Action next)
        //{
        //    IEditorCommandHandlerService commandHandlerService = _commandHandlerService;
        //    var nextCommandHandler = next;
        //    commandHandlerService.Execute<WordDeleteToEndCommandArgs>((Func<ITextView, ITextBuffer, WordDeleteToEndCommandArgs>)((view, buffer) => new WordDeleteToEndCommandArgs(view, buffer)), nextCommandHandler);
        //}

        //private void ExecuteWordDeleteToStartCommand(Action next)
        //{
        //    IEditorCommandHandlerService commandHandlerService = _commandHandlerService;
        //    var nextCommandHandler = next;
        //    commandHandlerService.Execute<WordDeleteToStartCommandArgs>((Func<ITextView, ITextBuffer, WordDeleteToStartCommandArgs>)((view, buffer) => new WordDeleteToStartCommandArgs(view, buffer)), nextCommandHandler);
        //}

        private void ExecuteLeftKeyCommand(Action next)
        {
            var commandHandlerService = _commandHandlerService;
            var nextCommandHandler = next;
            commandHandlerService.Execute((view, buffer) => new LeftKeyCommandArgs(view, buffer), nextCommandHandler);
        }

        //private void ExecuteRightKeyCommand(Action next)
        //{
        //    IEditorCommandHandlerService commandHandlerService = _commandHandlerService;
        //    var nextCommandHandler = next;
        //    commandHandlerService.Execute<RightKeyCommandArgs>((Func<ITextView, ITextBuffer, RightKeyCommandArgs>)((view, buffer) => new RightKeyCommandArgs(view, buffer)), nextCommandHandler);
        //}

        //private int QueryAutomaticLineEnderStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        //{
        //    return GetCommandState<AutomaticLineEnderCommandArgs>((Func<ITextView, ITextBuffer, AutomaticLineEnderCommandArgs>)((view, buffer) => new AutomaticLineEnderCommandArgs(view, buffer)), ref pguidCmdGroup, commandCount, prgCmds, commandText);
        //}

        //private int QueryBackspaceKeyStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        //{
        //    return GetCommandState<BackspaceKeyCommandArgs>((Func<ITextView, ITextBuffer, BackspaceKeyCommandArgs>)((view, buffer) => new BackspaceKeyCommandArgs(view, buffer)), ref pguidCmdGroup, commandCount, prgCmds, commandText);
        //}

        //private int QueryBackTabKeyStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        //{
        //    return GetCommandState<BackTabKeyCommandArgs>((Func<ITextView, ITextBuffer, BackTabKeyCommandArgs>)((view, buffer) => new BackTabKeyCommandArgs(view, buffer)), ref pguidCmdGroup, commandCount, prgCmds, commandText);
        //}

        //private int QueryCommentSelectionStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        //{
        //    return GetCommandState<CommentSelectionCommandArgs>((Func<ITextView, ITextBuffer, CommentSelectionCommandArgs>)((view, buffer) => new CommentSelectionCommandArgs(view, buffer)), ref pguidCmdGroup, commandCount, prgCmds, commandText);
        //}

        //private int QueryCommitUniqueCompletionListItemStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        //{
        //    return GetCommandState<CommitUniqueCompletionListItemCommandArgs>((Func<ITextView, ITextBuffer, CommitUniqueCompletionListItemCommandArgs>)((view, buffer) => new CommitUniqueCompletionListItemCommandArgs(view, buffer)), ref pguidCmdGroup, commandCount, prgCmds, commandText);
        //}

        //private int QueryCopyStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        //{
        //    return GetCommandState<CopyCommandArgs>((Func<ITextView, ITextBuffer, CopyCommandArgs>)((view, buffer) => new CopyCommandArgs(view, buffer)), ref pguidCmdGroup, commandCount, prgCmds, commandText);
        //}

        //private int QueryCopyToInteractiveStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        //{
        //    return GetCommandState<CopyToInteractiveCommandArgs>((Func<ITextView, ITextBuffer, CopyToInteractiveCommandArgs>)((view, buffer) => new CopyToInteractiveCommandArgs(view, buffer)), ref pguidCmdGroup, commandCount, prgCmds, commandText);
        //}

        //private int QueryCutStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        //{
        //    return GetCommandState<CutCommandArgs>((Func<ITextView, ITextBuffer, CutCommandArgs>)((view, buffer) => new CutCommandArgs(view, buffer)), ref pguidCmdGroup, commandCount, prgCmds, commandText);
        //}

        //private int QueryDeleteKeyStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        //{
        //    return GetCommandState<DeleteKeyCommandArgs>((Func<ITextView, ITextBuffer, DeleteKeyCommandArgs>)((view, buffer) => new DeleteKeyCommandArgs(view, buffer)), ref pguidCmdGroup, commandCount, prgCmds, commandText);
        //}

        //private int QueryDocumentEndStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        //{
        //    return GetCommandState<DocumentEndCommandArgs>((Func<ITextView, ITextBuffer, DocumentEndCommandArgs>)((view, buffer) => new DocumentEndCommandArgs(view, buffer)), ref pguidCmdGroup, commandCount, prgCmds, commandText);
        //}

        //private int QueryDocumentStartStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        //{
        //    return GetCommandState<DocumentStartCommandArgs>((Func<ITextView, ITextBuffer, DocumentStartCommandArgs>)((view, buffer) => new DocumentStartCommandArgs(view, buffer)), ref pguidCmdGroup, commandCount, prgCmds, commandText);
        //}

        //private int QueryDownKeyStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        //{
        //    return GetCommandState<DownKeyCommandArgs>((Func<ITextView, ITextBuffer, DownKeyCommandArgs>)((view, buffer) => new DownKeyCommandArgs(view, buffer)), ref pguidCmdGroup, commandCount, prgCmds, commandText);
        //}

        //private int QueryEncapsulateFieldStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        //{
        //    return GetCommandState<EncapsulateFieldCommandArgs>((Func<ITextView, ITextBuffer, EncapsulateFieldCommandArgs>)((view, buffer) => new EncapsulateFieldCommandArgs(view, buffer)), ref pguidCmdGroup, commandCount, prgCmds, commandText);
        //}

        //private int QueryEscapeKeyStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        //{
        //    return GetCommandState<EscapeKeyCommandArgs>((Func<ITextView, ITextBuffer, EscapeKeyCommandArgs>)((view, buffer) => new EscapeKeyCommandArgs(view, buffer)), ref pguidCmdGroup, commandCount, prgCmds, commandText);
        //}

        //private int QueryExecuteInInteractiveStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        //{
        //    return GetCommandState<ExecuteInInteractiveCommandArgs>((Func<ITextView, ITextBuffer, ExecuteInInteractiveCommandArgs>)((view, buffer) => new ExecuteInInteractiveCommandArgs(view, buffer)), ref pguidCmdGroup, commandCount, prgCmds, commandText);
        //}

        //private int QueryExtractInterfaceStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        //{
        //    return GetCommandState<ExtractInterfaceCommandArgs>((Func<ITextView, ITextBuffer, ExtractInterfaceCommandArgs>)((view, buffer) => new ExtractInterfaceCommandArgs(view, buffer)), ref pguidCmdGroup, commandCount, prgCmds, commandText);
        //}

        //private int QueryExtractMethodStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        //{
        //    return GetCommandState<ExtractMethodCommandArgs>((Func<ITextView, ITextBuffer, ExtractMethodCommandArgs>)((view, buffer) => new ExtractMethodCommandArgs(view, buffer)), ref pguidCmdGroup, commandCount, prgCmds, commandText);
        //}

        //private int QueryFindReferencesStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        //{
        //    return GetCommandState<FindReferencesCommandArgs>((Func<ITextView, ITextBuffer, FindReferencesCommandArgs>)((view, buffer) => new FindReferencesCommandArgs(view, buffer)), ref pguidCmdGroup, commandCount, prgCmds, commandText);
        //}

        //private int QueryFormatDocumentStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        //{
        //    return GetCommandState<FormatDocumentCommandArgs>((Func<ITextView, ITextBuffer, FormatDocumentCommandArgs>)((view, buffer) => new FormatDocumentCommandArgs(view, buffer)), ref pguidCmdGroup, commandCount, prgCmds, commandText);
        //}

        //private int QueryFormatSelectionStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        //{
        //    return GetCommandState<FormatSelectionCommandArgs>((Func<ITextView, ITextBuffer, FormatSelectionCommandArgs>)((view, buffer) => new FormatSelectionCommandArgs(view, buffer)), ref pguidCmdGroup, commandCount, prgCmds, commandText);
        //}

        //private int QueryGoToNextMemberStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        //{
        //    return GetCommandState<GoToNextMemberCommandArgs>((Func<ITextView, ITextBuffer, GoToNextMemberCommandArgs>)((view, buffer) => new GoToNextMemberCommandArgs(view, buffer)), ref pguidCmdGroup, commandCount, prgCmds, commandText);
        //}

        //private int QueryGoToPreviousMemberStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        //{
        //    return GetCommandState<GoToPreviousMemberCommandArgs>((Func<ITextView, ITextBuffer, GoToPreviousMemberCommandArgs>)((view, buffer) => new GoToPreviousMemberCommandArgs(view, buffer)), ref pguidCmdGroup, commandCount, prgCmds, commandText);
        //}

        //private int QueryGoToDefinitionStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        //{
        //    return GetCommandState<GoToDefinitionCommandArgs>((Func<ITextView, ITextBuffer, GoToDefinitionCommandArgs>)((view, buffer) => new GoToDefinitionCommandArgs(view, buffer)), ref pguidCmdGroup, commandCount, prgCmds, commandText);
        //}

        //private int QueryInsertCommentStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        //{
        //    return GetCommandState<InsertCommentCommandArgs>((Func<ITextView, ITextBuffer, InsertCommentCommandArgs>)((view, buffer) => new InsertCommentCommandArgs(view, buffer)), ref pguidCmdGroup, commandCount, prgCmds, commandText);
        //}

        //private int QueryInsertSnippetStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        //{
        //    return GetCommandState<InsertSnippetCommandArgs>((Func<ITextView, ITextBuffer, InsertSnippetCommandArgs>)((view, buffer) => new InsertSnippetCommandArgs(view, buffer)), ref pguidCmdGroup, commandCount, prgCmds, commandText);
        //}

        //private int QueryInvokeCompletionListStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        //{
        //    return GetCommandState<InvokeCompletionListCommandArgs>((Func<ITextView, ITextBuffer, InvokeCompletionListCommandArgs>)((view, buffer) => new InvokeCompletionListCommandArgs(view, buffer)), ref pguidCmdGroup, commandCount, prgCmds, commandText);
        //}

        //private int QueryInvokeQuickInfoStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        //{
        //    return GetCommandState<InvokeQuickInfoCommandArgs>((Func<ITextView, ITextBuffer, InvokeQuickInfoCommandArgs>)((view, buffer) => new InvokeQuickInfoCommandArgs(view, buffer)), ref pguidCmdGroup, commandCount, prgCmds, commandText);
        //}

        //private int QueryInvokeSignatureHelpStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        //{
        //    return GetCommandState<InvokeSignatureHelpCommandArgs>((Func<ITextView, ITextBuffer, InvokeSignatureHelpCommandArgs>)((view, buffer) => new InvokeSignatureHelpCommandArgs(view, buffer)), ref pguidCmdGroup, commandCount, prgCmds, commandText);
        //}

        //private int QueryLineEndStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        //{
        //    return GetCommandState<LineEndCommandArgs>((Func<ITextView, ITextBuffer, LineEndCommandArgs>)((view, buffer) => new LineEndCommandArgs(view, buffer)), ref pguidCmdGroup, commandCount, prgCmds, commandText);
        //}

        //private int QueryLineEndExtendStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        //{
        //    return GetCommandState<LineEndExtendCommandArgs>((Func<ITextView, ITextBuffer, LineEndExtendCommandArgs>)((view, buffer) => new LineEndExtendCommandArgs(view, buffer)), ref pguidCmdGroup, commandCount, prgCmds, commandText);
        //}

        //private int QueryLineStartStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        //{
        //    return GetCommandState<LineStartCommandArgs>((Func<ITextView, ITextBuffer, LineStartCommandArgs>)((view, buffer) => new LineStartCommandArgs(view, buffer)), ref pguidCmdGroup, commandCount, prgCmds, commandText);
        //}

        //private int QueryLineStartExtendStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        //{
        //    return GetCommandState<LineStartExtendCommandArgs>((Func<ITextView, ITextBuffer, LineStartExtendCommandArgs>)((view, buffer) => new LineStartExtendCommandArgs(view, buffer)), ref pguidCmdGroup, commandCount, prgCmds, commandText);
        //}

        //private int QueryMoveSelectedLinesDownStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        //{
        //    return GetCommandState<MoveSelectedLinesDownCommandArgs>((Func<ITextView, ITextBuffer, MoveSelectedLinesDownCommandArgs>)((view, buffer) => new MoveSelectedLinesDownCommandArgs(view, buffer)), ref pguidCmdGroup, commandCount, prgCmds, commandText);
        //}

        //private int QueryMoveSelectedLinesUpStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        //{
        //    return GetCommandState<MoveSelectedLinesUpCommandArgs>((Func<ITextView, ITextBuffer, MoveSelectedLinesUpCommandArgs>)((view, buffer) => new MoveSelectedLinesUpCommandArgs(view, buffer)), ref pguidCmdGroup, commandCount, prgCmds, commandText);
        //}

        //private int QueryNavigateToNextHighlightedReferenceStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        //{
        //    return GetCommandState<NavigateToNextHighlightedReferenceCommandArgs>((Func<ITextView, ITextBuffer, NavigateToNextHighlightedReferenceCommandArgs>)((view, buffer) => new NavigateToNextHighlightedReferenceCommandArgs(view, buffer)), ref pguidCmdGroup, commandCount, prgCmds, commandText);
        //}

        //private int QueryNavigateToPreviousHighlightedReferenceStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        //{
        //    return GetCommandState<NavigateToPreviousHighlightedReferenceCommandArgs>((Func<ITextView, ITextBuffer, NavigateToPreviousHighlightedReferenceCommandArgs>)((view, buffer) => new NavigateToPreviousHighlightedReferenceCommandArgs(view, buffer)), ref pguidCmdGroup, commandCount, prgCmds, commandText);
        //}

        //private int QueryOpenLineAboveStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        //{
        //    return GetCommandState<OpenLineAboveCommandArgs>((Func<ITextView, ITextBuffer, OpenLineAboveCommandArgs>)((view, buffer) => new OpenLineAboveCommandArgs(view, buffer)), ref pguidCmdGroup, commandCount, prgCmds, commandText);
        //}

        //private int QueryOpenLineBelowStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        //{
        //    return GetCommandState<OpenLineBelowCommandArgs>((Func<ITextView, ITextBuffer, OpenLineBelowCommandArgs>)((view, buffer) => new OpenLineBelowCommandArgs(view, buffer)), ref pguidCmdGroup, commandCount, prgCmds, commandText);
        //}

        //private int QueryPageDownKeyStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        //{
        //    return GetCommandState<PageDownKeyCommandArgs>((Func<ITextView, ITextBuffer, PageDownKeyCommandArgs>)((view, buffer) => new PageDownKeyCommandArgs(view, buffer)), ref pguidCmdGroup, commandCount, prgCmds, commandText);
        //}

        //private int QueryPageUpKeyStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        //{
        //    return GetCommandState<PageUpKeyCommandArgs>((Func<ITextView, ITextBuffer, PageUpKeyCommandArgs>)((view, buffer) => new PageUpKeyCommandArgs(view, buffer)), ref pguidCmdGroup, commandCount, prgCmds, commandText);
        //}

        //private int QueryPasteStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        //{
        //    return GetCommandState<PasteCommandArgs>((Func<ITextView, ITextBuffer, PasteCommandArgs>)((view, buffer) => new PasteCommandArgs(view, buffer)), ref pguidCmdGroup, commandCount, prgCmds, commandText);
        //}

        //private int QueryRedoStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        //{
        //    return GetCommandState<RedoCommandArgs>((Func<ITextView, ITextBuffer, RedoCommandArgs>)((view, buffer) => new RedoCommandArgs(view, buffer, 1)), ref pguidCmdGroup, commandCount, prgCmds, commandText);
        //}

        //private int QueryRemoveParametersStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        //{
        //    return GetCommandState<RemoveParametersCommandArgs>((Func<ITextView, ITextBuffer, RemoveParametersCommandArgs>)((view, buffer) => new RemoveParametersCommandArgs(view, buffer)), ref pguidCmdGroup, commandCount, prgCmds, commandText);
        //}

        //private int QueryRenameStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        //{
        //    return GetCommandState<RenameCommandArgs>((Func<ITextView, ITextBuffer, RenameCommandArgs>)((view, buffer) => new RenameCommandArgs(view, buffer)), ref pguidCmdGroup, commandCount, prgCmds, commandText);
        //}

        //private int QueryExpandSelectionStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        //{
        //    return GetCommandState<ExpandSelectionCommandArgs>((Func<ITextView, ITextBuffer, ExpandSelectionCommandArgs>)((view, buffer) => new ExpandSelectionCommandArgs(view, buffer)), ref pguidCmdGroup, commandCount, prgCmds, commandText);
        //}

        //private int QueryContractSelectionStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        //{
        //    return GetCommandState<ContractSelectionCommandArgs>((Func<ITextView, ITextBuffer, ContractSelectionCommandArgs>)((view, buffer) => new ContractSelectionCommandArgs(view, buffer)), ref pguidCmdGroup, commandCount, prgCmds, commandText);
        //}

        //private int QueryReorderParametersStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        //{
        //    return GetCommandState<ReorderParametersCommandArgs>((Func<ITextView, ITextBuffer, ReorderParametersCommandArgs>)((view, buffer) => new ReorderParametersCommandArgs(view, buffer)), ref pguidCmdGroup, commandCount, prgCmds, commandText);
        //}

        //private int QueryReturnKeyStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        //{
        //    return GetCommandState<ReturnKeyCommandArgs>((Func<ITextView, ITextBuffer, ReturnKeyCommandArgs>)((view, buffer) => new ReturnKeyCommandArgs(view, buffer)), ref pguidCmdGroup, commandCount, prgCmds, commandText);
        //}

        //private int QuerySaveStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        //{
        //    return GetCommandState<SaveCommandArgs>((Func<ITextView, ITextBuffer, SaveCommandArgs>)((view, buffer) => new SaveCommandArgs(view, buffer)), ref pguidCmdGroup, commandCount, prgCmds, commandText);
        //}

        //private int QuerySelectAllStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        //{
        //    return GetCommandState<SelectAllCommandArgs>((Func<ITextView, ITextBuffer, SelectAllCommandArgs>)((view, buffer) => new SelectAllCommandArgs(view, buffer)), ref pguidCmdGroup, commandCount, prgCmds, commandText);
        //}

        //private int QueryStartAutomaticOutliningStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        //{
        //    return GetCommandState<StartAutomaticOutliningCommandArgs>((Func<ITextView, ITextBuffer, StartAutomaticOutliningCommandArgs>)((view, buffer) => new StartAutomaticOutliningCommandArgs(view, buffer)), ref pguidCmdGroup, commandCount, prgCmds, commandText);
        //}

        //private int QuerySurroundWithStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        //{
        //    return GetCommandState<SurroundWithCommandArgs>((Func<ITextView, ITextBuffer, SurroundWithCommandArgs>)((view, buffer) => new SurroundWithCommandArgs(view, buffer)), ref pguidCmdGroup, commandCount, prgCmds, commandText);
        //}

        //private int QuerySyncClassViewStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        //{
        //    return GetCommandState<SyncClassViewCommandArgs>((Func<ITextView, ITextBuffer, SyncClassViewCommandArgs>)((view, buffer) => new SyncClassViewCommandArgs(view, buffer)), ref pguidCmdGroup, commandCount, prgCmds, commandText);
        //}

        //private int QueryTabKeyStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        //{
        //    return GetCommandState<TabKeyCommandArgs>((Func<ITextView, ITextBuffer, TabKeyCommandArgs>)((view, buffer) => new TabKeyCommandArgs(view, buffer)), ref pguidCmdGroup, commandCount, prgCmds, commandText);
        //}

        //private int QueryToggleCompletionModeStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        //{
        //    return GetCommandState<ToggleCompletionModeCommandArgs>((Func<ITextView, ITextBuffer, ToggleCompletionModeCommandArgs>)((view, buffer) => new ToggleCompletionModeCommandArgs(view, buffer)), ref pguidCmdGroup, commandCount, prgCmds, commandText);
        //}

        //private int QueryTypeCharStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText, char typedChar)
        //{
        //    return GetCommandState<TypeCharCommandArgs>((Func<ITextView, ITextBuffer, TypeCharCommandArgs>)((textView, subjectBuffer) => new TypeCharCommandArgs(TextView, subjectBuffer, typedChar)), ref pguidCmdGroup, commandCount, prgCmds, commandText);
        //}

        //private int QueryUncommentSelectionStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        //{
        //    return GetCommandState<UncommentSelectionCommandArgs>((Func<ITextView, ITextBuffer, UncommentSelectionCommandArgs>)((view, buffer) => new UncommentSelectionCommandArgs(view, buffer)), ref pguidCmdGroup, commandCount, prgCmds, commandText);
        //}

        //private int QueryUndoStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        //{
        //    return GetCommandState<UndoCommandArgs>((Func<ITextView, ITextBuffer, UndoCommandArgs>)((view, buffer) => new UndoCommandArgs(view, buffer, 1)), ref pguidCmdGroup, commandCount, prgCmds, commandText);
        //}

        //private int QueryUpKeyStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        //{
        //    return GetCommandState<UpKeyCommandArgs>((Func<ITextView, ITextBuffer, UpKeyCommandArgs>)((view, buffer) => new UpKeyCommandArgs(view, buffer)), ref pguidCmdGroup, commandCount, prgCmds, commandText);
        //}

        //private int QueryViewCallHierarchyStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        //{
        //    return GetCommandState<ViewCallHierarchyCommandArgs>((Func<ITextView, ITextBuffer, ViewCallHierarchyCommandArgs>)((view, buffer) => new ViewCallHierarchyCommandArgs(view, buffer)), ref pguidCmdGroup, commandCount, prgCmds, commandText);
        //}

        //private int QueryWordDeleteToEndStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        //{
        //    return GetCommandState<WordDeleteToEndCommandArgs>((Func<ITextView, ITextBuffer, WordDeleteToEndCommandArgs>)((view, buffer) => new WordDeleteToEndCommandArgs(view, buffer)), ref pguidCmdGroup, commandCount, prgCmds, commandText);
        //}

        //private int QueryWordDeleteToStartStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        //{
        //    return GetCommandState<WordDeleteToStartCommandArgs>((Func<ITextView, ITextBuffer, WordDeleteToStartCommandArgs>)((view, buffer) => new WordDeleteToStartCommandArgs(view, buffer)), ref pguidCmdGroup, commandCount, prgCmds, commandText);
        //}

        private int QueryLeftKeyStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        {
            return GetCommandState((view, buffer) => new LeftKeyCommandArgs(view, buffer), ref pguidCmdGroup, commandCount, prgCmds, commandText);
        }

        //private int QueryRightKeyStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        //{
        //    return GetCommandState<RightKeyCommandArgs>((Func<ITextView, ITextBuffer, RightKeyCommandArgs>)((view, buffer) => new RightKeyCommandArgs(view, buffer)), ref pguidCmdGroup, commandCount, prgCmds, commandText);
        //}

        private int GetCommandState<T>(Func<ITextView, ITextBuffer, T> argsFactory, ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText) where T : EditorCommandArgs
        {
            var guidCmdGroup = pguidCmdGroup;
            var commandState = _commandHandlerService.GetCommandState(argsFactory, () =>
            {
                NextCommandTarget.QueryStatus(ref guidCmdGroup, commandCount, prgCmds, commandText);
                return new CommandState(((int)prgCmds[0].cmdf & 2) == 2, ((int)prgCmds[0].cmdf & 4) == 4, Utilities.Utilities.GetCmdText(commandText));
            });
            if (!commandState.IsAvailable)
                return 1;
            var olecmdf1 = commandState.IsAvailable ? Olecmdf.OlecmdfEnabled : Olecmdf.OlecmdfInvisible;
            var olecmdf2 = commandState.IsChecked ? Olecmdf.OlecmdfLatched : Olecmdf.OlecmdfNinched;
            prgCmds[0].cmdf = (uint)(olecmdf1 | olecmdf2 | Olecmdf.OlecmdfSupported);
            if (!string.IsNullOrEmpty(commandState.DisplayText) && Utilities.Utilities.GetCmdText(commandText) != commandState.DisplayText)
                Utilities.Utilities.SetCmdText(commandText, commandState.DisplayText);
            return 0;
        }
    }

    public enum Olecmdf
    {
        OlecmdfSupported = 1,
        OlecmdfEnabled = 2,
        OlecmdfLatched = 4,
        OlecmdfNinched = 8,
        OlecmdfInvisible = 16, // 0x00000010
        OlecmdfDefhideonctxtmenu = 32, // 0x00000020
    }

    public interface IEditorCommandHandlerService
    {
        CommandState GetCommandState<T>(Func<ITextView, ITextBuffer, T> argsFactory, Func<CommandState> nextCommandHandler) where T : EditorCommandArgs;

        void Execute<T>(Func<ITextView, ITextBuffer, T> argsFactory, Action nextCommandHandler) where T : EditorCommandArgs;
    }

    public abstract class EditorCommandArgs : CommandArgs
    {
        public ITextBuffer SubjectBuffer { get; }

        public ITextView TextView { get; }

        protected EditorCommandArgs(ITextView textView, ITextBuffer subjectBuffer)
        {
            var textView1 = textView;
            TextView = textView1 ?? throw new ArgumentNullException(nameof(textView));
            var textBuffer = subjectBuffer;
            SubjectBuffer = textBuffer ?? throw new ArgumentNullException(nameof(subjectBuffer));
        }
    }

    public abstract class CommandArgs
    {
    }

    public sealed class LeftKeyCommandArgs : EditorCommandArgs
    {
        public LeftKeyCommandArgs(ITextView textView, ITextBuffer subjectBuffer)
            : base(textView, subjectBuffer)
        {
        }
    }

    public struct CommandState
    {
        public bool IsUnspecified { get; }

        public bool IsAvailable { get; }

        public bool IsChecked { get; }

        public string DisplayText { get; }

        public CommandState(bool isAvailable = false, bool isChecked = false, string displayText = null, bool isUnspecified = false)
        {
            if (isUnspecified && (isAvailable | isChecked || displayText != null))
                throw new ArgumentException("Unspecified command state cannot be combined with other states or command text.");
            IsAvailable = isAvailable;
            IsChecked = isChecked;
            IsUnspecified = isUnspecified;
            DisplayText = displayText;
        }

        public static CommandState Available { get; } = new CommandState(true);

        public static CommandState Unavailable { get; } = new CommandState(false);

        public static CommandState Unspecified { get; } = new CommandState(false, false, null, true);
    }


    internal class EditorCommandHandlerService : IEditorCommandHandlerService
    {
        public CommandState GetCommandState<T>(Func<ITextView, ITextBuffer, T> argsFactory, Func<CommandState> nextCommandHandler) where T : EditorCommandArgs
        {
            throw new NotImplementedException();
        }

        public void Execute<T>(Func<ITextView, ITextBuffer, T> argsFactory, Action nextCommandHandler) where T : EditorCommandArgs
        {
            throw new NotImplementedException();
        }
    }


    public interface IEditorCommandHandlerServiceFactory
    {
        IEditorCommandHandlerService GetService(ITextView textView);

        IEditorCommandHandlerService GetService(ITextView textView, ITextBuffer subjectBuffer);
    }


    [Export(typeof(IEditorCommandHandlerServiceFactory))]
    internal class EditorCommandHandlerServiceFactory : IEditorCommandHandlerServiceFactory
    {
        private readonly IEnumerable<Lazy<ICommandHandler, ICommandHandlerMetadata>> _commandHandlers;
        private readonly IList<Lazy<ICommandingTextBufferResolverProvider, IContentTypeMetadata>> _bufferResolverProviders;
        private readonly IUiThreadOperationExecutor _uiThreadOperationExecutor;
        private readonly IContentTypeRegistryService _contentTypeRegistryService;
        private readonly IGuardedOperations _guardedOperations;
        private readonly StableContentTypeComparer _contentTypeComparer;

        [ImportingConstructor]
        public EditorCommandHandlerServiceFactory(
            [ImportMany] IEnumerable<Lazy<ICommandHandler, ICommandHandlerMetadata>> commandHandlers,
            [ImportMany] IEnumerable<Lazy<ICommandingTextBufferResolverProvider, IContentTypeMetadata>> bufferResolvers, 
            IUiThreadOperationExecutor uiThreadOperationExecutor, 
            //JoinableTaskContext joinableTaskContext, 
            IContentTypeRegistryService contentTypeRegistryService, 
            IGuardedOperations guardedOperations)
        {
            _commandHandlers = OrderCommandHandlers(commandHandlers);
        }

        public IEditorCommandHandlerService GetService(ITextView textView)
        {
            var bufferResolverProvider = _guardedOperations.InvokeBestMatchingFactory(_bufferResolverProviders,
                textView.TextBuffer.ContentType, _contentTypeRegistryService, this);

            var bufferResolver = (ICommandingTextBufferResolver) null;

            _guardedOperations.CallExtensionPoint(() => bufferResolver = bufferResolverProvider.CreateResolver(textView));

            bufferResolver = bufferResolver ?? new DefaultBufferResolver(textView);

            return null;

            //return new EditorCommandHandlerService(textView, _commandHandlers, _uiThreadOperationExecutor,
            //    /*this._joinableTaskContext,*/ _contentTypeComparer, bufferResolver, _guardedOperations);
        }

        public IEditorCommandHandlerService GetService(ITextView textView, ITextBuffer subjectBuffer)
        {
            if (subjectBuffer == null)
                return GetService(textView);
            throw new NotImplementedException("Maybe this will not be implemented at all");
        }

        private IEnumerable<Lazy<ICommandHandler, ICommandHandlerMetadata>> OrderCommandHandlers(IEnumerable<Lazy<ICommandHandler, ICommandHandlerMetadata>> commandHandlers)
        {
            var source = commandHandlers;
            var contentTypeComparer = _contentTypeComparer;
            return source.OrderBy(handler => handler.Metadata.ContentTypes, contentTypeComparer);
        }
    }

    public interface ICommandHandler
    {
    }

    public interface ICommandHandlerMetadata : IOrderable, IContentTypeMetadata
    {
        [DefaultValue(null)]
        IEnumerable<string> TextViewRoles { get; }
    }

    public sealed class CommandBindingDefinition
    {
    }

    public interface ICommandBindingMetadata
    {
        string[] CommandSet { get; }

        uint[] CommandId { get; }

        string[] CommandArgsType { get; }
    }
}