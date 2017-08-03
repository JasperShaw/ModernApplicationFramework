namespace ModernApplicationFramework.Extended.Settings.Keyboard
{
    public partial class KeyboardSettingsView
    {
        public KeyboardSettingsView()
        {
            InitializeComponent();
            Loaded += KeyboardSettingsView_Loaded            ;
        }

        private void KeyboardSettingsView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            CommandsListBox.SelectedIndex = 0;
        }
    }
}
