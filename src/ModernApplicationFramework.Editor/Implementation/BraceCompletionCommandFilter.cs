using System;
using System.Runtime.InteropServices;
using ModernApplicationFramework.Editor.Interop;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Text;

namespace ModernApplicationFramework.Editor.Implementation
{
    internal class BraceCompletionCommandFilter : ICommandTarget
    {
        private ICommandTarget _nextCommandFilter;
        private SimpleTextViewWindow _simpleTextViewWindow;
        private IBraceCompletionManager _manager;
        //private ICompletionBroker _completionBroker;

        internal ICommandTarget NextCommandFilter
        {
            get => _nextCommandFilter;
            set => _nextCommandFilter = value;
        }

        //TODO: Completion broker
        //private ICompletionBroker CompletionBroker
        //{
        //    get
        //    {
        //        if (_completionBroker == null)
        //            _completionBroker = EditorParts.ComponentModel.GetService<ICompletionBroker>();
        //        return _completionBroker;
        //    }
        //}

        //private bool IsCompletionActive => CompletionBroker.IsCompletionActive(View);

        private IBraceCompletionManager Manager
        {
            get
            {
                if (_manager == null && !View.Properties.TryGetProperty("BraceCompletionManager", out _manager))
                    _manager = null;
                return _manager;
            }
        }

        private ITextView View => _simpleTextViewWindow.TextViewHost.TextView;

        private bool Enabled => Manager != null && Manager.Enabled;

        public BraceCompletionCommandFilter(SimpleTextViewWindow simpleTextViewWindow)
        {
            _simpleTextViewWindow = simpleTextViewWindow;
        }

        public int QueryStatus(Guid commandGroup, uint commandCount, Olecmd[] commands, IntPtr pCmdText)
        {
            return _nextCommandFilter.QueryStatus(commandGroup, commandCount, commands, pCmdText);
        }

        public int Exec(Guid commandGroup, uint commandId, uint nCmdexecopt, IntPtr input, IntPtr output)
        {
            if (_nextCommandFilter == null)
                return -2147467259;
            if (Enabled && commandGroup == MafConstants.EditorCommandGroup)
            {
                if (commandId == 1)
                {
                    var forNativeVariant = (char) (ushort) Marshal.GetObjectForNativeVariant(input);
                    if (Manager.HasActiveSessions && Manager.ClosingBraces.IndexOf(forNativeVariant) > -1 || Manager.OpeningBraces.IndexOf(forNativeVariant) > -1)
                    {
                        Manager.PreTypeChar(forNativeVariant, out var handledCommand);
                        if (handledCommand)
                            return 0;
                        var num = _nextCommandFilter.Exec(commandGroup, commandId, nCmdexecopt, input, output);
                        Manager.PostTypeChar(forNativeVariant);
                        return num;
                    }
                }
                else if (Manager.HasActiveSessions)
                {
                    switch (commandId)
                    {
                        case 2:
                            Manager.PreBackspace(out var handledCommand1);
                            if (handledCommand1)
                                return 0;
                            var num1 = _nextCommandFilter.Exec(commandGroup, commandId, nCmdexecopt, input, output);
                            Manager.PostBackspace();
                            return num1;
                        case 3:
                            //TODO: Completion broker
                            //if (!this.IsCompletionActive)
                            {
                                Manager.PreReturn(out var handledCommand2);
                                if (handledCommand2)
                                    return 0;
                                int num2 = _nextCommandFilter.Exec(commandGroup, commandId, nCmdexecopt, input, output);
                                Manager.PostReturn();
                                return num2;
                            }
                            break;
                        case 4:
                            //TODO: Completion broker
                            //if (!this.IsCompletionActive)
                            {
                                Manager.PreTab(out var handledCommand2);
                                if (handledCommand2)
                                    return 0;
                                int num2 = _nextCommandFilter.Exec(commandGroup, commandId, nCmdexecopt, input, output);
                                Manager.PostTab();
                                return num2;
                            }
                            break;
                        case 6:
                            Manager.PreDelete(out var handledCommand3);
                            if (handledCommand3)
                                return 0;
                            int num3 = _nextCommandFilter.Exec(commandGroup, commandId, nCmdexecopt, input, output);
                            Manager.PostDelete();
                            return num3;
                    }
                }
            }
            return _nextCommandFilter.Exec(commandGroup, commandId, nCmdexecopt, input, output);
        }
    }
}
