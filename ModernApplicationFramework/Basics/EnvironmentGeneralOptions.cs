using System.ComponentModel;
using System.Runtime.CompilerServices;
using ModernApplicationFramework.Annotations;

namespace ModernApplicationFramework.Basics
{
	public sealed class EnvironmentGeneralOptions : INotifyPropertyChanged
	{
		public static EnvironmentGeneralOptions Instance { get; private set; }


		private bool _useTitleCaseOnMenu;

		public bool UseTitleCaseOnMenu
		{
			get => _useTitleCaseOnMenu;
			set
			{
				if (value == _useTitleCaseOnMenu) return;
				_useTitleCaseOnMenu = value;
				OnPropertyChanged();
			}
		}

		public EnvironmentGeneralOptions()
		{
			Instance = this;
		}

		public void Load()
		{
			UseTitleCaseOnMenu = Properties.Settings.Default.UseTitleCaseOnMenu;
		}

		public void Save()
		{
			Properties.Settings.Default.UseTitleCaseOnMenu = UseTitleCaseOnMenu;
			Properties.Settings.Default.Save();
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		private void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
