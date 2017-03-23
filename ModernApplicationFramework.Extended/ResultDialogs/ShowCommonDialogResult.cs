using System;
using Caliburn.Micro;
using Microsoft.Win32;

namespace ModernApplicationFramework.Extended.ResultDialogs
{
    public class ShowCommonDialogResult : IResult
    {
        private readonly CommonDialog _commonDialog;
        public event EventHandler<ResultCompletionEventArgs> Completed;

        public ShowCommonDialogResult(CommonDialog commonDialog)
        {
            _commonDialog = commonDialog;
        }

        public void Execute(CoroutineExecutionContext context)
        {
            var result = _commonDialog.ShowDialog().GetValueOrDefault(false);
            Completed?.Invoke(this, new ResultCompletionEventArgs {WasCancelled = !result});
        }
    }
}