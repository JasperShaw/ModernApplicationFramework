using System.ComponentModel.Composition;
using ToolBar = ModernApplicationFramework.Controls.ToolBar;

namespace ModernApplicationFramework.MVVM.Demo.Toolbars
{
    [Export(typeof(ToolBar))]
    public partial class DemoToolbar
    {
        public DemoToolbar()
        {
            InitializeComponent();
            DataContext = new DemoToolbarModel();
        }
    }
}
