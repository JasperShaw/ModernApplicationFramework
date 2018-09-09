using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Threading;
using ModernApplicationFramework.Basics.Threading;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Threading;

namespace ModernApplicationFramework.Editor.Implementation
{
    [Export(typeof(IExtensionErrorHandler))]
    internal class ExtensionErrorHandler : IExtensionErrorHandler
    {
        private bool _exceptionsEncountered;

        [Import]
        private JoinableTaskContext JoinableTaskContext { get; set; }

        public void HandleError(object sender, Exception exception)
        {
            JoinableTaskContext.Factory.StartOnIdle(async () =>
            {
                await JoinableTaskContext.Factory.SwitchToMainThreadAsync();
                if (_exceptionsEncountered)
                    return;
                ReportExceptions();
            });


            if (_exceptionsEncountered)
                return;
            _exceptionsEncountered = true;
            Application.Current.Dispatcher.BeginInvoke(new Action(ReportExceptions), DispatcherPriority.ApplicationIdle, Array.Empty<object>());
        }

        private void ReportExceptions()
        {
            MessageBox.Show("Extension error handler reports: Something broke!");
        }
    }
}
