using System.ComponentModel.Composition;
using System.Windows;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Core.DialogProiver
{
	[Export(typeof(IDialogProvider))]
	public class DialogProvider : IDialogProvider
	{
		public void ShowMessage(string message)
		{
			MessageBox.Show(message, "Modern Application Framework", MessageBoxButton.OK,
				MessageBoxImage.None, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
		}

		public bool Ask(string message, MessageBoxButton button)
		{
			MessageBoxResult result = MessageBox.Show(message, "Modern Application Framework", button,
				MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.DefaultDesktopOnly);
			return result.HasFlag(MessageBoxResult.OK);
		}

		public void ShowError(string message)
		{
			MessageBox.Show(message, "Modern Application Framework", MessageBoxButton.OK,
				MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
		}

		public void Inform(string message)
		{
			MessageBox.Show(message, "Modern Application Framework", MessageBoxButton.OK,
				MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
		}

		public void Warn(string message)
		{
			MessageBox.Show(message, "Modern Application Framework", MessageBoxButton.OK,
				MessageBoxImage.Exclamation, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
		}
	}
}
