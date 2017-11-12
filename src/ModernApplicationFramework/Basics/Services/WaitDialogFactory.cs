using System.ComponentModel.Composition;
using ModernApplicationFramework.Controls.Dialogs;
using ModernApplicationFramework.Interfaces.Controls;
using ModernApplicationFramework.Interfaces.Services;

namespace ModernApplicationFramework.Basics.Services
{
    [Export(typeof(IWaitDialogFactory))]
    internal class WaitDialogFactory : IWaitDialogFactory
    {
        /// <summary>
        /// Creates an instance of an <see cref="IWaitDialog"/>.
        /// </summary>
        /// <param name="waitDialog">The wait dialog.</param>
        public void CreateInstance(out IWaitDialog waitDialog)
        {
            waitDialog = new WaitDialog();
        }
    }
}