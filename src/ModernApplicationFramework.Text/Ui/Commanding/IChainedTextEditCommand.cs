﻿using System;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Text.Ui.Commanding
{
    public interface IChainedTextEditCommand<in T> : ITextEditCommand, INamed where T : CommandArgs
    {
        void ExecuteCommand(T args, Action nextCommandHandler, CommandExecutionContext executionContext);
        CommandState GetCommandState(T args, Func<CommandState> nextCommandHandler);
    }
}