using System;
using System.Threading;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.Controls;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Text.Utilities;

namespace ModernApplicationFramework.Editor.Implementation
{
    internal sealed class WaitContext : IWaitContext
    {
        private readonly string _title;
        private string _message;
        private bool _allowCancel;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly IWaitDialog _dialog;

        public CancellationToken CancellationToken => !_allowCancel ? CancellationToken.None : _cancellationTokenSource.Token;

        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                UpdateDialog();
            }
        }

        public bool AllowCancel
        {
            get => _allowCancel;
            set
            {
                _allowCancel = value;
                UpdateDialog();
            }
        }

        public WaitContext(IWaitDialogFactory dialogFactory, string title, string message, bool allowCancel)
        {
            _title = title;
            _message = message;
            _allowCancel = allowCancel;
            _cancellationTokenSource = new CancellationTokenSource();
            _dialog = CreateDialog(dialogFactory);
        }

        public IWaitDialog CreateDialog(IWaitDialogFactory factory)
        {
            factory.CreateInstance(out var waitDialog);
            if (waitDialog == null)
                throw new InvalidOperationException();
            waitDialog.StartWaitDialogWithCallback(_title, _title, null, null, _allowCancel, 2, false, 0, 0,
                new Callback(this));
            return waitDialog;
        }

        public void UpdateProgress()
        {
        }

        public void Dispose()
        {
            _dialog.EndWaitDialog(out _);
        }

        private void OnCanceled()
        {
            if (!_allowCancel)
                return;
            _cancellationTokenSource.Cancel();
        }

        private void UpdateDialog()
        {
            _dialog.UpdateProgress(_message, null, null, 0, 0, !_allowCancel, out _);
        }

        private class Callback : IWaitDialogCallback
        {
            private readonly WaitContext _waitContext;

            public Callback(WaitContext waitContext)
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