using System;
using System.Threading;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.Controls;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Text.Utilities;

namespace ModernApplicationFramework.Editor.Implementation
{
    internal sealed class UiThreadOperationContext : AbstractUiThreadOperationContext
    {
        private readonly string _title;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly IWaitDialog _dialog;
        private bool _disposed;

        public UiThreadOperationContext(IWaitDialogFactory waitDialogFactory , string title, string defaultDescription, bool allowCancellation, bool showProgress)
        : base(allowCancellation, defaultDescription)
        {
            _title = title;
            _cancellationTokenSource = new CancellationTokenSource();
            _dialog = CreateDialog(waitDialogFactory, showProgress);
        }

        private IWaitDialog CreateDialog(IWaitDialogFactory waitDialogFactory, bool showProgress)
        {
            waitDialogFactory.CreateInstance(out var waitDialog);
            if (waitDialog == null)
                throw new InvalidOperationException();
            var callback = new Callback(this);
            waitDialog.StartWaitDialogWithCallback(_title, Description, null, null, AllowCancellation, 2, showProgress,
                TotalItems, CompletedItems, callback);
            return waitDialog;
        }

        public override void Dispose()
        {
            if (_disposed)
                return;
            _disposed = true;
            _dialog.EndWaitDialog(out _);
        }

        public override CancellationToken UserCancellationToken => !AllowCancellation ? CancellationToken.None : _cancellationTokenSource.Token;

        public override void TakeOwnership()
        {
            Dispose();
        }

        protected override void OnScopesChanged()
        {
            UpdateDialog();
        }

        protected override void OnScopeProgressChanged(IUiThreadOperationScope scope)
        {
            UpdateDialog();
        }

        private void UpdateDialog()
        {
            if (_disposed)
                return;
            _dialog.UpdateProgress(Description, null, null, CompletedItems, TotalItems, !AllowCancellation, out _);
        }

        private void OnCanceled()
        {
            if (!AllowCancellation)
                return;
            _cancellationTokenSource.Cancel();
        }

        private class Callback : IWaitDialogCallback
        {
            private readonly UiThreadOperationContext _waitContext;

            public Callback(UiThreadOperationContext waitContext)
            {
                _waitContext = waitContext;
            }

            public void OnCanceled()
            {
                _waitContext.OnCanceled();
            }
        }
    }
}