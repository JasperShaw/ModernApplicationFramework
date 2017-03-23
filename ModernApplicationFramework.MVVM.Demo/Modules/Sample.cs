using System.ComponentModel.Composition;
using System.Windows;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.MVVM.Demo.Modules.Document;

namespace ModernApplicationFramework.MVVM.Demo.Modules
{
    [Export(typeof(ISample))]
    public class Sample : ISample
    {
        public string Name => "Filter Designer";

        public void Activate(IDockingHostViewModel shell)
        {
            if (shell == null)
                MessageBox.Show("WTF");
            //shell?.ShowTool<IOutput>();
        }
    }
}
