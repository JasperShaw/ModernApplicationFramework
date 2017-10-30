using System.ComponentModel.Composition;
using ToolBar = ModernApplicationFramework.Controls.ToolBar;

namespace ModernApplicationFramework.MVVM.Controls.Toolbars
{
    /// <summary>
    /// Interaktionslogik für TestToolbarView.xaml
    /// </summary>
    [Export(typeof(ToolBar))]
    public partial class StandardToolbar
    {
        public StandardToolbar()
        {
            InitializeComponent();
            DataContext = new StandardToolbarModel();
        }
    }
}
