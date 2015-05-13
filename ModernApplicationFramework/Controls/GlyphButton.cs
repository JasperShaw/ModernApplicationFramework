using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ModernApplicationFramework.Controls
{
	public class GlyphButton : Button
	{
		static GlyphButton()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(GlyphButton), new FrameworkPropertyMetadata(typeof(GlyphButton)));
		}

		public static readonly DependencyProperty GlyphDataProperty = DependencyProperty.Register(
			"GlyphData", typeof (Geometry), typeof (GlyphButton), new PropertyMetadata(default(Geometry)));

		public Geometry GlyphData
		{
			get { return (Geometry) GetValue(GlyphDataProperty); }
			set { SetValue(GlyphDataProperty, value); }
		}
	}
}
