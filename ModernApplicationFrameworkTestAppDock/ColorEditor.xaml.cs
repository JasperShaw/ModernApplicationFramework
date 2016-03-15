using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;
using ModernApplicationFramework.Core.Themes;

namespace ModernApplicationFrameworkTestAppDock
{
	/// <summary>
	/// Interaction logic for ColorEditor.xaml
	/// </summary>
	public partial class ColorEditor
	{
		public ColorEditor()
		{
			InitializeComponent();
			ListView.ContextMenu = ListView.Resources["ContextMenuColor"] as ContextMenu;
			LoadColors(string.Empty);

		    var m = Application.Current.MainWindow as MainWindow;
            if (m != null)
                m.OnThemeChanged += OnThemeChanged;

        }

        private void OnThemeChanged(object sender, EventArgs e)
        {
            LoadColors(string.Empty);
        }

        private void LoadColors(string filterBy)
		{
			var themeColors = new List<ThemeColor>();
			var type = typeof (EnvironmentColors);

			IEnumerable<PropertyInfo> properties = type.GetProperties(BindingFlags.Public | BindingFlags.Static);

			foreach (var property in properties)
			{
				var themeColor = new ThemeColor {ColorKey = property.Name};

				var k = property.GetValue(this, null) as ComponentResourceKey;			
				try
				{
					var brush = (Brush)FindResource(k);
					if (brush is SolidColorBrush)
					{
						var solidBrush = brush as SolidColorBrush;
						Color color = solidBrush.Color;
						if (string.IsNullOrWhiteSpace(filterBy) || String.CompareOrdinal(String.Format("{0},{1},{2}", color.R.ToString(CultureInfo.InvariantCulture), color.G.ToString(CultureInfo.InvariantCulture), color.B.ToString(CultureInfo.InvariantCulture)), filterBy) == 0)
						{
							themeColor.ComponentResourceKey = k;
							themeColor.Color = solidBrush;
							themeColor.RGB = $"{color.A.ToString("X2")},{color.R.ToString("X2")},{color.G.ToString("X2")},{color.B.ToString("X2")}";
							themeColor.FontColor = GetLabelFontColor(Color.FromArgb(color.A, color.R, color.G, color.B));
							themeColors.Add(themeColor);
						}
					}
				}
				catch (Exception)
				{
				}
				
			}
			ListView.ItemsSource = themeColors;
		}

		public Brush GetLabelFontColor(Color backgroundColor)
		{
			var c = backgroundColor;
			var l = 0.2126 * c.ScR + 0.7152 * c.ScG + 0.0722 * c.ScB;
			if (l < 0.5)
			{
				return Brushes.White;
			}
			return Brushes.Black;
		}

		private void ButtonChange_Click(object sender, RoutedEventArgs e)
		{
			Resources.Clear();


			foreach (var item in ListView.Items)
			{
				var i = item as ThemeColor;
				{
					if (i == null)
						continue;
					i.RGB = i.RGB.Replace(",", "");
					var convertFromString = ColorConverter.ConvertFromString("#" + i.RGB);
					if (convertFromString != null)
					{
						Color color = (Color)convertFromString;
						Application.Current.Resources[i.ComponentResourceKey] = new SolidColorBrush(color);
					}
				}
			}
			LoadColors(string.Empty);
		}

		private void ButtonCreate_Click(object sender, RoutedEventArgs e)
		{
			SaveFileDialog dlg = new SaveFileDialog {FileName = "Colors", DefaultExt = ".txt"};

			bool? result = dlg.ShowDialog();

			if (result != true)
				return;

			var filepath = dlg.FileName;

			foreach (var i in from object item in ListView.Items select item as ThemeColor)
			{
				using (StreamWriter file = new StreamWriter(filepath, true))
				{
					file.WriteLine(i.ColorKey + "		" + i.RGB);
				}
			}


		}	

		private void ButtonRefresh_Click(object sender, RoutedEventArgs e)
		{
			LoadColors(string.Empty);
		}

		private void MenuItemCopy_Click(object sender, RoutedEventArgs e)
		{
			var themeColor = ListView.SelectedItem as ThemeColor;
			if (themeColor != null)
			{
				string colorKey = themeColor.RGB;
				Clipboard.SetText(colorKey);
				MessageBox.Show(string.Format("Copied '{0}' to the clipboard.", colorKey), "Copy To Clipboard", MessageBoxButton.OK,
								MessageBoxImage.Information);
			}
		}

		private void ListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var item = ListView.SelectedItem as ThemeColor;
			if (item == null)
				return;

			var a = GetColor(item.RGB, ColorPart.A);
			var r = GetColor(item.RGB, ColorPart.R);
			var g = GetColor(item.RGB, ColorPart.G);
			var b = GetColor(item.RGB, ColorPart.B);

			TextBoxAlpha.Text = a;
			TextBoxRed.Text = r;
			TextBoxGreen.Text = g;
			TextBoxBlue.Text = b;
		}

		private string GetColor(string rgb, ColorPart color)
		{
			rgb = rgb.Replace(",", "");
			switch (color)
			{
				case ColorPart.A:
					return rgb.Remove(2, rgb.Length-2);
				case ColorPart.R:
					return rgb.Substring(2, 2);
				case ColorPart.G:
					return rgb.Substring(4, 2);
				default:
					return rgb.Substring(6);
			}
		}

		private enum ColorPart
		{
			A,R,G,B
		}

		private void TextBoxColor_OnTextChanged(object sender, TextChangedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(TextBoxAlpha.Text) || string.IsNullOrWhiteSpace(TextBoxRed.Text)  || string.IsNullOrWhiteSpace(TextBoxGreen.Text)|| string.IsNullOrWhiteSpace(TextBoxBlue.Text))
				return;
			try
			{
				var convertFromString = ColorConverter.ConvertFromString("#" + TextBoxAlpha.Text + TextBoxRed.Text + TextBoxGreen.Text + TextBoxBlue.Text);
				if (convertFromString == null)
					return;
				var color = (Color)convertFromString;
				PreviewBorder.Background = new SolidColorBrush(color);

				var l = ListView.SelectedItem as ThemeColor;
				if (l != null)
					l.RGB = color.ToString().Remove(0,1);
			}
			catch (Exception)
			{
			}
			
		}
	}

	public class ThemeColor
	{
		public ComponentResourceKey ComponentResourceKey { get; set; }
		public string ColorKey { get; set; }
		public string RGB { get; set; }
		public Brush Color { get; set; }
		public Brush FontColor { get; set; }
	}
}
