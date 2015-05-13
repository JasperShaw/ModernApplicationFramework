using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using ModernApplicationFramework.Controls;

namespace ModernApplicationFramework.Docking.Controls
{
	class DropDownTitleBarButton : DropDownButton, INonClientArea
	{
		static DropDownTitleBarButton()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(DropDownTitleBarButton), new FrameworkPropertyMetadata(typeof(DropDownTitleBarButton)));
		}
		public int HitTest(Point point)
		{
			return 1;
		}
	}
}
