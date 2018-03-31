using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Caliburn.Micro;

namespace ModernApplicationFramework.Extended.Demo.Modules.FilterDesigner.ViewModels
{
    public class OutputConnectorViewModel : ConnectorViewModel
    {
        private readonly Func<BitmapSource> _valueCallback;
        private readonly BindableCollection<ConnectionViewModel> _connections;

        public override ConnectorDirection ConnectorDirection => ConnectorDirection.Output;

        public IObservableCollection<ConnectionViewModel> Connections => _connections;

        public BitmapSource Value => _valueCallback();

        public OutputConnectorViewModel(ElementViewModel element, string name, Color color, Func<BitmapSource> valueCallback)
            : base(element, name, color)
        {
            _connections = new BindableCollection<ConnectionViewModel>();
            _valueCallback = valueCallback;
        }
    }
}
