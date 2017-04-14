namespace ModernApplicationFramework.Basics.CustomizeDialog.Views
{
    public partial class AddCommandDialogView
    {
        public AddCommandDialogView()
        {
            InitializeComponent();
            Loaded += AddCommandDialogView_Loaded;
        }

        private void AddCommandDialogView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            CategoriesListView.Focus();
        }
    }
}
