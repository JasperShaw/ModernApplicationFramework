using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Threading;
using ModernApplicationFramework.Controls.Dialogs;
using ModernApplicationFramework.Extended.Core.LayoutItems;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Extended.Demo.Modules.WaitingWindow
{
    [DisplayName("Wait Dialog")]
    [Export(typeof(WaitingDialogDemoViewModel))]
    public sealed class WaitingDialogDemoViewModel : LayoutItem
    {
        public ICommand ActionCommand => new Command(ActionMode);

        public ICommand MafCancelCommand => new Command(MafTaskHelperCancel);
        public ICommand MafCommand => new Command(MafTaskHelperNormal);

        public ICommand ManualCommand => new Command(ManualMode);

        public ICommand UpdateMessageCommand => new Command(UpdateMessage);

        [ImportingConstructor]
        public WaitingDialogDemoViewModel()
        {
            DisplayName = "Wait Dialog";
        }

        private static async Task AsyncMethod(IProgress<WaitDialogProgressData> progress)
        {
            await Task.Delay(5000);
            MessageBox.Show("Completed");
        }

        private static async Task AsyncMethod(CancellationToken token)
        {
            try
            {
                await Task.Delay(5000, token);
                MessageBox.Show("Completed");
            }
            catch (TaskCanceledException)
            {
            }
        }

        private void ActionMode()
        {
            var f = IoC.Get<IWaitDialogFactory>(); ;
            f.CreateInstance(out var window);
            window.SetAction(NormalMethod);
            window.StartWaitDialog("Wait", "Wait", "Wait", string.Empty, 2, false, true);
        }


        private async Task AsyncMethodCancel(IProgress<WaitDialogProgressData> progress,
            CancellationToken cancellationToken)
        {
            await Task.Delay(5000, cancellationToken);
            MessageBox.Show("Completed");
        }

        private void MafTaskHelperCancel()
        {
            MafTaskHelper.Run("Wait", AsyncMethodCancel);
        }


        private void MafTaskHelperNormal()
        {
            MafTaskHelper.Run("Wait", AsyncMethod);
        }

        private async void ManualMode()
        {
            var f = IoC.Get<IWaitDialogFactory>();
            f.CreateInstance(out var window);

            var cancellationTokenSource = new CancellationTokenSource();
            window.StartWaitDialog("Wait", "Wait", "Wait", string.Empty, 2, true, true);
            window.StartWaitDialogWithCallback("Wait", "Wait", "Wait", string.Empty, true, 2, true, 0, 0,
                new PrivateCallback(cancellationTokenSource));
            await Task.Run(() => AsyncMethod(cancellationTokenSource.Token), cancellationTokenSource.Token);
            window.EndWaitDialog(out var canceled);
            if (canceled)
                MessageBox.Show("Canceled");
        }

        private void NormalMethod()
        {
            try
            {
                Thread.Sleep(5000);
                MessageBox.Show("Completed");
            }
            catch (ThreadInterruptedException)
            {
            }
        }

        private void UpdateMessage()
        {
            MafTaskHelper.Run("Wait", UpdatingMethodCancel, TimeSpan.FromSeconds(0));
        }


        private async Task UpdatingMethodCancel(IProgress<WaitDialogProgressData> progress,
            CancellationToken cancellationToken)
        {
            progress.Report(new WaitDialogProgressData("Wait", string.Empty, "Working", true, 0, 5));
            await Task.Delay(1000, cancellationToken);
            progress.Report(new WaitDialogProgressData("Wait", "1", "Working", true, 1, 5));
            await Task.Delay(1000, cancellationToken);
            progress.Report(new WaitDialogProgressData("Wait", "2", "Working", true, 2, 5));
            await Task.Delay(1000, cancellationToken);
            progress.Report(new WaitDialogProgressData("Wait", "3", "Working", true, 3, 5));
            await Task.Delay(1000, cancellationToken);
            progress.Report(new WaitDialogProgressData("Wait", "4", "Working", true, 4, 5));
            await Task.Delay(1000, cancellationToken);
            progress.Report(new WaitDialogProgressData("Wait", "5", "Working", true, 5, 5));
            MessageBox.Show("Completed");
        }

        private class PrivateCallback : IWaitDialogCallback
        {
            private readonly CancellationTokenSource _cancellationSource;

            internal PrivateCallback(CancellationTokenSource cancellationSource)
            {
                Validate.IsNotNull(cancellationSource, nameof(cancellationSource));
                _cancellationSource = cancellationSource;
            }

            public void OnCanceled()
            {
                _cancellationSource.Cancel();
            }
        }
    }
}