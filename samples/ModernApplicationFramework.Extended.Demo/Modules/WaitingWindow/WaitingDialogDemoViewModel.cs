using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.VisualStudio.Threading;
using ModernApplicationFramework.Controls.Dialogs;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Extended.Demo.Modules.WaitingWindow
{
    [DisplayName("Wait Dialog")]
    [Export(typeof(WaitingDialogDemoViewModel))]
    public sealed class WaitingDialogDemoViewModel : Core.LayoutItems.LayoutItem
    {
        public ICommand ShowWaitDialogCommand => new Command(ShowDialog);

        public ICommand ShowCancelWaitDialogCommand => new Command(ShowCancelDialog);

        private void ShowDialog()
        {


            Func<IProgress<WaitDialogProgressData>, CancellationToken, Task> asyncMethod = AsyncMethod;


            var sesion = WaitDialogHelper.CreateSession();

            var d = new WaitDialog(sesion.Callback);
            d.ShowDialog(() => asyncMethod(sesion.Progress, sesion.UserCancellationToken));

            //Func<IProgress<WaitDialogProgressData>, CancellationToken, Task> asyncMethod = AsyncMethod;


            //IWaitDialogFactory f = new WaitDialogFactory();
            //var session = f.StartWaitDialog("Message", null, TimeSpan.FromSeconds(2.0));

            //try
            //{
            //    var t = new Thread(() => { });
            //    t.IsBackground = true;

            //    JoinableTaskFactory fa = new JoinableTaskFactory(new JoinableTaskContext(t));

            //    fa.RunAsync(() => asyncMethod(session.Progress, session.UserCancellationToken));

            //    //var t = new Task(() => asyncMethod(session.Progress), session.UserCancellationToken);
            //    //t.Start();
            //}
            //finally
            //{
            //    session?.Dispose();
            //}

        }

        private async Task AsyncMethod(IProgress<WaitDialogProgressData> progress, CancellationToken cancellationToken)
        {
            await Task.Delay(5000, cancellationToken);
            MessageBox.Show("Test");
        }

        private void ShowCancelDialog()
        {
        }

        [ImportingConstructor]
        public WaitingDialogDemoViewModel()
        {
            DisplayName = "Wait Dialog";
        }
    }
}
