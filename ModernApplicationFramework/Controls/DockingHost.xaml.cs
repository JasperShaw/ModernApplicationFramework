using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ModernApplicationFramework.Controls
{
	/// <summary>
	/// Interaction logic for DockingHost.xaml
	/// </summary>
	public partial class DockingHost
	{
		public DockingHost()
		{
			InitializeComponent();
		}

		private void DockingManager_OnLoaded(object sender, RoutedEventArgs e)
		{
			throw new NotImplementedException();
		}

		public void Connect(int connectionId, object target)
		{
			throw new NotImplementedException();
		}
	}
}
