/************************************************************************

   AvalonDock

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the New BSD
   License (BSD) as published at http://avalondock.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up AvalonDock in Extended WPF Toolkit Plus at http://xceed.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like facebook.com/datagrids

  **********************************************************************/

using System;
using System.Globalization;
using System.Windows;

namespace ModernApplicationFramework.Docking.Layout
{
    [Serializable]
    public abstract class LayoutPositionableGroup<T> : LayoutGroup<T>, ILayoutPositionableElement,
        ILayoutPositionableElementWithActualSize where T : class, ILayoutElement
    {
        private static readonly GridLengthConverter _gridLengthConverter = new GridLengthConverter();
        [NonSerialized] private double _actualHeight;
        [NonSerialized] private double _actualWidth;
        private GridLength _dockHeight = new GridLength(1.0, GridUnitType.Star);
        private double _dockMinHeight = 25.0;
        private double _dockMinWidth = 25.0;
        private GridLength _dockWidth = new GridLength(1.0, GridUnitType.Star);
        private double _floatingHeight;
        private double _floatingLeft;
        private double _floatingTop;
        private double _floatingWidth;
        private bool _isMaximized;

        public double FloatingHeight
        {
            get { return _floatingHeight; }
            set
            {
                if (_floatingHeight == value)
                    return;
                RaisePropertyChanging("FloatingHeight");
                _floatingHeight = value;
                RaisePropertyChanged("FloatingHeight");
            }
        }

        public double FloatingLeft
        {
            get { return _floatingLeft; }
            set
            {
                if (_floatingLeft == value)
                    return;
                RaisePropertyChanging("FloatingLeft");
                _floatingLeft = value;
                RaisePropertyChanged("FloatingLeft");
            }
        }

        public double FloatingTop
        {
            get { return _floatingTop; }
            set
            {
                if (_floatingTop != value)
                {
                    RaisePropertyChanging("FloatingTop");
                    _floatingTop = value;
                    RaisePropertyChanged("FloatingTop");
                }
            }
        }

        public double FloatingWidth
        {
            get { return _floatingWidth; }
            set
            {
                if (_floatingWidth != value)
                {
                    RaisePropertyChanging("FloatingWidth");
                    _floatingWidth = value;
                    RaisePropertyChanged("FloatingWidth");
                }
            }
        }

        public bool IsMaximized
        {
            get { return _isMaximized; }
            set
            {
                if (_isMaximized != value)
                {
                    _isMaximized = value;
                    RaisePropertyChanged("IsMaximized");
                }
            }
        }

        public GridLength DockHeight
        {
            get { return _dockHeight; }
            set
            {
                if (DockHeight != value)
                {
                    RaisePropertyChanging("DockHeight");
                    _dockHeight = value;
                    RaisePropertyChanged("DockHeight");

                    OnDockHeightChanged();
                }
            }
        }

        public double DockMinHeight
        {
            get { return _dockMinHeight; }
            set
            {
                if (_dockMinHeight == value)
                    return;
                MathHelper.AssertIsPositiveOrZero(value);
                RaisePropertyChanging("DockMinHeight");
                _dockMinHeight = value;
                RaisePropertyChanged("DockMinHeight");
            }
        }

        public double DockMinWidth
        {
            get { return _dockMinWidth; }
            set
            {
                if (_dockMinWidth != value)
                {
                    MathHelper.AssertIsPositiveOrZero(value);
                    RaisePropertyChanging("DockMinWidth");
                    _dockMinWidth = value;
                    RaisePropertyChanged("DockMinWidth");
                }
            }
        }

        public GridLength DockWidth
        {
            get { return _dockWidth; }
            set
            {
                if (DockWidth == value)
                    return;
                RaisePropertyChanging("DockWidth");
                _dockWidth = value;
                RaisePropertyChanged("DockWidth");

                OnDockWidthChanged();
            }
        }

        double ILayoutPositionableElementWithActualSize.ActualHeight
        {
            get { return _actualHeight; }
            set { _actualHeight = value; }
        }

        double ILayoutPositionableElementWithActualSize.ActualWidth
        {
            get { return _actualWidth; }
            set { _actualWidth = value; }
        }

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            if (reader.MoveToAttribute("DockWidth"))
                _dockWidth = (GridLength) _gridLengthConverter.ConvertFromInvariantString(reader.Value);
            if (reader.MoveToAttribute("DockHeight"))
                _dockHeight = (GridLength) _gridLengthConverter.ConvertFromInvariantString(reader.Value);

            if (reader.MoveToAttribute("DocMinWidth"))
                _dockMinWidth = double.Parse(reader.Value, CultureInfo.InvariantCulture);
            if (reader.MoveToAttribute("DocMinHeight"))
                _dockMinHeight = double.Parse(reader.Value, CultureInfo.InvariantCulture);

            if (reader.MoveToAttribute("FloatingWidth"))
                _floatingWidth = double.Parse(reader.Value, CultureInfo.InvariantCulture);
            if (reader.MoveToAttribute("FloatingHeight"))
                _floatingHeight = double.Parse(reader.Value, CultureInfo.InvariantCulture);
            if (reader.MoveToAttribute("FloatingLeft"))
                _floatingLeft = double.Parse(reader.Value, CultureInfo.InvariantCulture);
            if (reader.MoveToAttribute("FloatingTop"))
                _floatingTop = double.Parse(reader.Value, CultureInfo.InvariantCulture);
            if (reader.MoveToAttribute("IsMaximized"))
                _isMaximized = bool.Parse(reader.Value);

            base.ReadXml(reader);
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            if (DockWidth.Value != 1.0 || !DockWidth.IsStar)
                writer.WriteAttributeString("DockWidth", _gridLengthConverter.ConvertToInvariantString(DockWidth));
            if (DockHeight.Value != 1.0 || !DockHeight.IsStar)
                writer.WriteAttributeString("DockHeight", _gridLengthConverter.ConvertToInvariantString(DockHeight));

            if (DockMinWidth != 25.0)
                writer.WriteAttributeString("DocMinWidth", DockMinWidth.ToString(CultureInfo.InvariantCulture));
            if (DockMinHeight != 25.0)
                writer.WriteAttributeString("DockMinHeight", DockMinHeight.ToString(CultureInfo.InvariantCulture));

            if (FloatingWidth != 0.0)
                writer.WriteAttributeString("FloatingWidth", FloatingWidth.ToString(CultureInfo.InvariantCulture));
            if (FloatingHeight != 0.0)
                writer.WriteAttributeString("FloatingHeight", FloatingHeight.ToString(CultureInfo.InvariantCulture));
            if (FloatingLeft != 0.0)
                writer.WriteAttributeString("FloatingLeft", FloatingLeft.ToString(CultureInfo.InvariantCulture));
            if (FloatingTop != 0.0)
                writer.WriteAttributeString("FloatingTop", FloatingTop.ToString(CultureInfo.InvariantCulture));
            if (IsMaximized)
                writer.WriteAttributeString("IsMaximized", IsMaximized.ToString());

            base.WriteXml(writer);
        }

        protected virtual void OnDockHeightChanged()
        {
        }

        protected virtual void OnDockWidthChanged()
        {
        }

        protected virtual void SetXmlAttributeValue(string name, string valueString)
        {
            switch (name)
            {
                case "DockWidth":
                    _dockWidth = (GridLength) _gridLengthConverter.ConvertFromInvariantString(valueString);
                    break;
                case "DockHeight":
                    _dockHeight = (GridLength) _gridLengthConverter.ConvertFromInvariantString(valueString);
                    break;
                case "DocMinWidth":
                    _dockMinWidth = double.Parse(valueString, CultureInfo.InvariantCulture);
                    break;
                case "DocMinHeight":
                    _dockMinHeight = double.Parse(valueString, CultureInfo.InvariantCulture);
                    break;
                case "FloatingWidth":
                    _floatingWidth = double.Parse(valueString, CultureInfo.InvariantCulture);
                    break;
                case "FloatingHeight":
                    _floatingHeight = double.Parse(valueString, CultureInfo.InvariantCulture);
                    break;
                case "FloatingLeft":
                    _floatingLeft = double.Parse(valueString, CultureInfo.InvariantCulture);
                    break;
                case "FloatingTop":
                    _floatingTop = double.Parse(valueString, CultureInfo.InvariantCulture);
                    break;
                default:
                    //base.SetXmlAttributeValue(name, valueString);
                    break;
            }
        }
    }
}