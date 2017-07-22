using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Controls.Dialogs
{
    internal sealed class MessageViewModel : ObservableObject
    {
        private MessageDialogCommand _response;
        private bool _confirmationState;

        public string Title { get; }

        public string Message { get; }

        public string ConfirmationMessage { get; }

        public bool ConfirmationState
        {
            get => _confirmationState;
            set => SetProperty(ref _confirmationState, value);
        }

        public MessageDialogCommand Response
        {
            get => _response;
            private set => SetProperty(ref _response, value);
        }

        public bool CanCancel { get; }

        public IEnumerable<ICommand> Commands { get; }

        internal ICommand DefaultCommand
        {
            get
            {
                return Commands.FirstOrDefault(c => ((Command)c).IsDefault);
            }
        }

        internal ICommand CancelCommand
        {
            get
            {
                return Commands.FirstOrDefault(c => ((Command)c).IsCancel);
            }
        }

        internal event EventHandler RequestClose;

        internal MessageViewModel(string title, string message, MessageDialogCommandSet commandSet)
            : this(title, message, commandSet, null)
        {
        }

        internal MessageViewModel(string title, string message, MessageDialogCommandSet commandSet, string confirmationMessage)
        {
            Validate.IsNotNullAndNotEmpty(title, "title");
            Validate.IsNotNullAndNotEmpty(message, "message");
            Title = title;
            Message = message;
            ConfirmationMessage = confirmationMessage;
            MessageDialogCommand unhandledResponse;
            Commands = CreateCommandsForSet(commandSet, out unhandledResponse);
            Response = unhandledResponse;
            CanCancel = CancelCommand != null;
        }

        private ICommand[] CreateCommandsForSet(MessageDialogCommandSet commandSet, out MessageDialogCommand unhandledResponse)
        {
            switch (commandSet)
            {
                case MessageDialogCommandSet.Ok:
                    unhandledResponse = MessageDialogCommand.Ok;
                    return new[]
                    {
                        CreateCommand(MessageDialogCommand.Ok, true, true)
                    };
                case MessageDialogCommandSet.OkCancel:
                    unhandledResponse = MessageDialogCommand.Cancel;
                    return new[]
                    {
                        CreateCommand(MessageDialogCommand.Ok, true),
                        CreateCommand(MessageDialogCommand.Cancel, false, true)
                    };
                case MessageDialogCommandSet.AbortRetryIgnore:
                    unhandledResponse = MessageDialogCommand.Ignore;
                    return new[]
                    {
                        CreateCommand(MessageDialogCommand.Abort),
                        CreateCommand(MessageDialogCommand.Retry),
                        CreateCommand(MessageDialogCommand.Ignore, true)
                    };
                case MessageDialogCommandSet.YesNoCancel:
                    unhandledResponse = MessageDialogCommand.Cancel;
                    return new[]
                    {
                        CreateCommand(MessageDialogCommand.Yes, true),
                        CreateCommand(MessageDialogCommand.No),
                        CreateCommand(MessageDialogCommand.Cancel, false, true)
                    };
                case MessageDialogCommandSet.YesNo:
                    unhandledResponse = MessageDialogCommand.No;
                    return new[]
                    {
                        CreateCommand(MessageDialogCommand.Yes, true),
                        CreateCommand(MessageDialogCommand.No, false, true)
                    };
                case MessageDialogCommandSet.RetryCancel:
                    unhandledResponse = MessageDialogCommand.Cancel;
                    return new[]
                    {
                        CreateCommand(MessageDialogCommand.Retry, true),
                        CreateCommand(MessageDialogCommand.Cancel, false, true)
                    };
                default:
                    throw new NotSupportedException(DialogResourcesResources.InvalidMessageDialogCommandSet);
            }
        }

        private ICommand CreateCommand(MessageDialogCommand command, bool isDefault = false, bool isCancel = false)
        {
            void Execute() => OnCommandExecuted(command);
            string name;
            switch (command)
            {
                case MessageDialogCommand.Ok:
                    name = DialogResourcesResources.DialogButton_Ok;
                    break;
                case MessageDialogCommand.Cancel:
                    name = DialogResourcesResources.DialogButton_Cancel;
                    break;
                case MessageDialogCommand.Abort:
                    name = DialogResourcesResources.DialogButton_Abort;
                    break;
                case MessageDialogCommand.Retry:
                    name = DialogResourcesResources.DialogButton_Retry;
                    break;
                case MessageDialogCommand.Ignore:
                    name = DialogResourcesResources.DialogButton_Ignore;
                    break;
                case MessageDialogCommand.Yes:
                    name = DialogResourcesResources.DialogButton_Yes;
                    break;
                case MessageDialogCommand.No:
                    name = DialogResourcesResources.DialogButton_No;
                    break;
                default:
                    throw new NotSupportedException(DialogResourcesResources.InvalidMessageDialogCommand);
            }
            return new Command(name, Execute, isDefault, isCancel);
        }

        private void OnCommandExecuted(MessageDialogCommand command)
        {
            Response = command;
            RequestClose.RaiseEvent(this);
        }


        private class Command : DelegateCommand
        {
            public string Name { get; }

            public bool IsDefault { get; }

            public bool IsCancel { get; }

            public Command(string name, Action execute, bool isDefault, bool isCancel)
                : base(arg => execute())
            {
                Validate.IsNotNullAndNotEmpty(name, "name");
                Name = name;
                IsDefault = isDefault;
                IsCancel = isCancel;
            }
        }
    }
}
