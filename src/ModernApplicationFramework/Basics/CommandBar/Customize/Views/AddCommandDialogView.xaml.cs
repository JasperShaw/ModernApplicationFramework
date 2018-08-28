using System.Windows;

namespace ModernApplicationFramework.Basics.CommandBar.Customize.Views
{
    public partial class AddCommandDialogView
    {
        public AddCommandDialogView()
        {
            InitializeComponent();
            Loaded += AddCommandDialogView_Loaded;}

        private void AddCommandDialogView_Loaded(object sender, RoutedEventArgs e)
        {
            CategoriesListView.Focus();
        }
    }
}