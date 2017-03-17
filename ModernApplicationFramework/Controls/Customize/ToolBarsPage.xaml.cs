using ModernApplicationFramework.ViewModels;

namespace ModernApplicationFramework.Controls.Customize
{
    /// <summary>
    ///     Interaktionslogik für ToolBarsPage.xaml
    /// </summary>
    public partial class ToolBarsPage
    {
        public ToolBarsPage()
        {
            InitializeComponent();
            DataContext = new ToolBarCustomizeDialogViewModel(this);
        }
    }
}