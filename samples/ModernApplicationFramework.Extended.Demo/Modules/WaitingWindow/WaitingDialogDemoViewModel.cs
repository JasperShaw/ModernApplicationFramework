using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Services.WaitDialog;
using ModernApplicationFramework.Basics.Threading;
using ModernApplicationFramework.Extended.Layout;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Threading;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Extended.Demo.Modules.WaitingWindow
{
    [DisplayName("Wait Dialog")]
    [Export(typeof(WaitingDialogDemoViewModel))]
    public sealed class WaitingDialogDemoViewModel : LayoutItem
    {
        public ICommand MafCancelCommand => new Command(MafTaskHelperCancel);

        public ICommand MafCommand => new Command(MafTaskHelperNormal);

        public ICommand ManualCommand => new Command(ManualMode);

        [ImportingConstructor]
        public WaitingDialogDemoViewModel()
        {
            DisplayName = "Wait Dialog";
        }

        private static async Task AsyncMethod2(IProgress<WaitDialogProgressData> progress, CancellationToken token)
        {
            try
            {
                progress.Report(new WaitDialogProgressData("Wait", string.Empty, "Working", true, 0, 5));
                await Task.Delay(1000, token);
                progress.Report(new WaitDialogProgressData("Wait", "1", "Working", true, 1, 5));
                await Task.Delay(1000, token);
                progress.Report(new WaitDialogProgressData("Wait", "2", "Working", true, 2, 5));
                await Task.Delay(1000, token);
                progress.Report(new WaitDialogProgressData("Wait", "3", "Working", true, 3, 5));
                await Task.Delay(1000, token);
                progress.Report(new WaitDialogProgressData("Wait", "4", "Working", true, 4, 5));
                await Task.Delay(1000, token);
                progress.Report(new WaitDialogProgressData("Wait", "5", "Working", true, 5, 5));
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
            window.StartWaitDialog("Wait", "Wait", "Wait", string.Empty, 2, false, true);
        }


        private void MafTaskHelperCancel()
        {
            ThreadHelper.JoinableTaskFactory.Run("Wait", async (_, token) =>
            {
                try
                {
                    await Task.Delay(10000, token);
                    MessageBox.Show("Completed");
                }
                catch (TaskCanceledException)
                {
                    MessageBox.Show("Canceled");
                }
            });
        }


        private void MafTaskHelperNormal()
        {
            ThreadHelper.JoinableTaskFactory.Run("Wait", async progress =>
            {
                var data = new WaitDialogProgressData("Wait", "Counting", null, false, 0, 10);
                progress.Report(data);
                for (int i = 0; i < 10; i++)
                {
                    await Task.Delay(1000);
                    progress.Report(data.NextStep());
                }
                MessageBox.Show("Completed");
            });
        }

        private async void ManualMode()
        {
            var f = IoC.Get<IWaitDialogFactory>();
            f.CreateInstance(out var window);

            var cancellationTokenSource = new CancellationTokenSource();
            window.StartWaitDialogWithCallback("Wait", "Wait", "Wait", string.Empty, true, 2, true, 5, 0,
                new PrivateCallback(cancellationTokenSource));
            await Task.Run(() => AsyncMethod2(WaitDialogHelper.CreateSession(window).Progress, cancellationTokenSource.Token), cancellationTokenSource.Token);
            window.EndWaitDialog(out var canceled);
            if (canceled)
                MessageBox.Show("Canceled");
        }

 
        private async Task UpdatingMethodCancel(IProgress<WaitDialogProgressData> progress,
            CancellationToken cancellationToken)
        {
            try
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
            catch (TaskCanceledException)
            {
                MessageBox.Show("Canceled");
            }
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