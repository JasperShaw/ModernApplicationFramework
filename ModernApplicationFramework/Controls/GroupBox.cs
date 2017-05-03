using System.Windows;
using ModernApplicationFramework.Controls.Utilities;

namespace ModernApplicationFramework.Controls
{
	public class GroupBox : System.Windows.Controls.GroupBox
	{

		public static readonly DependencyProperty CornerRadiusProperty
			= DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(GroupBox),
				new FrameworkPropertyMetadata(
					new CornerRadius(),
					FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender), IsCornerRadiusValid);

		public CornerRadius CornerRadius
		{
			get => (CornerRadius)GetValue(CornerRadiusProperty);
			set => SetValue(CornerRadiusProperty, value);
		}

		static GroupBox()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(GroupBox), new FrameworkPropertyMetadata(typeof(GroupBox)));
		}

		private static bool IsCornerRadiusValid(object value)
		{
			var cr = (CornerRadius)value;
			return (cr.IsValid(false, false, false, false));
		}
	}
}
