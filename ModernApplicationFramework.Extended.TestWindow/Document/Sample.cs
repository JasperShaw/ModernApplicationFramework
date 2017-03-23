using System.ComponentModel.Composition;
using System.Windows;
using ModernApplicationFramework.Extended.Interfaces;

namespace ModernApplicationFramework.Extended.TestWindow.Document
{
    [Export(typeof(ISample))]
    public class Sample : ISample
    {
        public string Name => "Filter Designer";

        public void Activate(IDockingHostViewModel shell)
        {
            if (shell == null)
                MessageBox.Show("WTF");
        }
    }
}
