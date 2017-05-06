using System.Windows;

namespace ModernApplicationFramework.Interfaces
{
	public interface IDialogProvider
	{
		void ShowMessage(string message);

		bool Ask(string message, MessageBoxButton button);

		void ShowError(string message);

		void Inform(string message);

		void Warn(string message);
	}
}