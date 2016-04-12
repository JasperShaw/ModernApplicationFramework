using System;
using ModernApplicationFramework.MVVM.Core.NativeMethods;

namespace ModernApplicationFramework.MVVM.Views
{
    /// <summary>
    /// Interaktionslogik für SettingsWindowView.xaml
    /// </summary>
    public partial class SettingsWindowView
    {
        public SettingsWindowView()
        {
            InitializeComponent();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            NativeMethods.RemoveIcon(this);
        }
    }
}
