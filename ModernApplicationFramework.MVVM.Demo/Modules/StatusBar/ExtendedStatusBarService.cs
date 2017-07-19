using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Services;
using ModernApplicationFramework.Interfaces.Services;

namespace ModernApplicationFramework.MVVM.Demo.Modules.StatusBar
{
    [Export(typeof(IStatusBarDataModelService))]
    public class ExtendedStatusBarService : AbstractStatusBarService, IStatusBarDataModel2
    {
        private string _infoTextA;
        private string _infoTextB;
        private string _infoTextC;

        public string InfoTextA
        {
            get => _infoTextA;
            set
            {
                if (value == _infoTextA) return;
                _infoTextA = value;
                OnPropertyChanged();
            }
        }

        public string InfoTextB
        {
            get => _infoTextB;
            set
            {
                if (value == _infoTextB) return;
                _infoTextB = value;
                OnPropertyChanged();
            }
        }

        public string InfoTextC
        {
            get => _infoTextC;
            set
            {
                if (value == _infoTextC) return;
                _infoTextC = value;
                OnPropertyChanged();
            }
        }

        protected override void SetTextInternal(int index, string text)
        {
            switch (index)
            {
                case 0:
                    Text = text;
                    break;
                case 1:
                    InfoTextA = text;
                    break;
                case 2:
                    InfoTextB = text;
                    break;
                case 3:
                    InfoTextC = text;
                    break;
            }
        }

        protected override string GetTextInternal(int index)
        {
            switch (index)
            {
                case 0:
                    return Text;
                case 1:
                    return InfoTextA;
                case 2:
                    return InfoTextB;
                case 3:
                    return InfoTextC;
            }
            return string.Empty;
        }
    }
}
