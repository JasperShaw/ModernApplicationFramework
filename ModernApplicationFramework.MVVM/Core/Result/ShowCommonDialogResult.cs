using System;
using Microsoft.Win32;
using ModernApplicationFramework.Caliburn;
using ModernApplicationFramework.Caliburn.Extensions;
using ModernApplicationFramework.Caliburn.Result;

namespace ModernApplicationFramework.MVVM.Core.Result
{
    public class ShowCommonDialogResult : IResult
    {
        public event EventHandler<ResultCompletionEventArgs> Completed;

        private readonly CommonDialog _commonDialog;

        public ShowCommonDialogResult(CommonDialog commonDialog)
        {
            _commonDialog = commonDialog;
        }

        public void Execute(CoroutineExecutionContext context)
        {
            var result = _commonDialog.ShowDialog().GetValueOrDefault(false);
            Completed(this, new ResultCompletionEventArgs { WasCancelled = !result });
        }
    }
}
