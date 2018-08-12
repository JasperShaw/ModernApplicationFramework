using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Threading;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Editor
{
    [Export(typeof(IExtensionErrorHandler))]
    internal class ExtensionErrorHandler : IExtensionErrorHandler
    {
        private bool _exceptionsEncountered;

        public void HandleError(object sender, Exception exception)
        {
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
