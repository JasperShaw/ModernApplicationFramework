using System.ComponentModel;
using ModernApplicationFramework.ViewModels;

namespace ModernApplicationFramework.Controls.Customize
{
    /// <summary>
    /// Interaktionslogik für ToolBarsPage.xaml
    /// </summary>
    public partial class ToolBarsPage
    {
        public ToolBarsPage()
        {
            InitializeComponent();
            DataContext = new ToolBarCustomizeDialogViewModel(this);

            if (DesignerProperties.GetIsInDesignMode(this))
                return;
        }

        public void Connect(int connectionId, object target)
        {
        }
    }
}
