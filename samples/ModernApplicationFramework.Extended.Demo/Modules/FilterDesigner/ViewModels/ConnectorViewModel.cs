using System;
using System.Windows;
using System.Windows.Media;
using Caliburn.Micro;

namespace ModernApplicationFramework.Extended.Demo.Modules.FilterDesigner.ViewModels
{
    public enum ConnectorDataType
    {

    }

    public enum ConnectorDirection
    {
        Input,
        Output
    }

    public abstract class ConnectorViewModel : PropertyChangedBase
    {
        public event EventHandler PositionChanged;

        private Point _position;

        public Color Color { get; } = Colors.Black;

        public abstract ConnectorDirection ConnectorDirection { get; }

        public ElementViewModel Element { get; }

        public string Name { get; }

        public Point Position
        {
            get => _position;
            set
            {
                _position = value;
                NotifyOfPropertyChange(() => Position);
                RaisePositionChanged();
            }
        }

        protected ConnectorViewModel(ElementViewModel element, string name, Color color)
        {
            Element = element;
            Name = name;
            Color = color;
        }

        private void RaisePositionChanged()
        {
            var handler = PositionChanged;
            handler?.Invoke(this, EventArgs.Empty);
        }
    }
}
