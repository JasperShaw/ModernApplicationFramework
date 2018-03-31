using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Caliburn.Micro;

namespace ModernApplicationFramework.Extended.Demo.Modules.FilterDesigner.ViewModels
{
    public abstract class ElementViewModel : PropertyChangedBase
    {
        public event EventHandler OutputChanged;

        public const double PreviewSize = 100;

        private double _x;
        private double _y;
        private string _name;
        private bool _isSelected;
        private readonly BindableCollection<InputConnectorViewModel> _inputConnectors;
        private OutputConnectorViewModel _outputConnector;

        [Browsable(false)]
        public double X
        {
            get => _x;
            set
            {
                _x = value;
                NotifyOfPropertyChange(() => X);
            }
        }

        public double Y
        {
            get => _y;
            set
            {
                _y = value;
                NotifyOfPropertyChange(() => Y);
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                NotifyOfPropertyChange(() => Name);
            }
        }

        [Browsable(false)]
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                NotifyOfPropertyChange(() => IsSelected);
            }
        }

        public abstract BitmapSource PreviewImage { get; }

        public IList<InputConnectorViewModel> InputConnectors => _inputConnectors;

        public OutputConnectorViewModel OutputConnector
        {
            get => _outputConnector;
            set
            {
                _outputConnector = value;
                NotifyOfPropertyChange(() => OutputConnector);
            }
        }

        public IEnumerable<ConnectionViewModel> AttachedConnections
        {
            get
            {
                return _inputConnectors.Select(x => x.Connection)
                    .Union(_outputConnector.Connections)
                    .Where(x => x != null);
            }
        }

        protected ElementViewModel()
        {
            _inputConnectors = new BindableCollection<InputConnectorViewModel>();
            _name = GetType().Name;
        }

        protected void AddInputConnector(string name, Color color)
        {
            var inputConnector = new InputConnectorViewModel(this, name, color);
            inputConnector.SourceChanged += (sender, e) => OnInputConnectorConnectionChanged();
            _inputConnectors.Add(inputConnector);
        }

        protected void SetOutputConnector(string name, Color color, Func<BitmapSource> valueCallback)
        {
            OutputConnector = new OutputConnectorViewModel(this, name, color, valueCallback);
        }

        protected virtual void OnInputConnectorConnectionChanged()
        {

        }

        protected virtual void RaiseOutputChanged()
        {
            EventHandler handler = OutputChanged;
            handler?.Invoke(this, EventArgs.Empty);
        }
    }
}
