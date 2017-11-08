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
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;
using System.Xml.Serialization;
using ModernApplicationFramework.Core.Comparers;

namespace ModernApplicationFramework.Docking.Layout
{
    [ContentProperty("Content")]
    [Serializable]
    public abstract class LayoutContent : LayoutElement, IXmlSerializable, ILayoutElementForFloatingWindow,
        IComparable<LayoutContent>, ILayoutPreviousContainer
    {
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof (string), typeof (LayoutContent),
                new UIPropertyMetadata(null, OnTitlePropertyChanged, CoerceTitleValue));

        private bool _canClose = true;
        private bool _canFloat = true;

        private readonly LogicalStringComparer _comparer = new LogicalStringComparer();
        [NonSerialized] private object _content;
        private string _contentId;
        private double _floatingHeight;
        private double _floatingLeft;
        private double _floatingTop;
        private double _floatingWidth;
        private ImageSource _iconSource;
        [field: NonSerialized] private bool _isActive;
        private bool _isLastFocusedDocument;
        private bool _isMaximized;
        private bool _isSelected;
        private DateTime? _lastActivationTimeStamp;
        [field: NonSerialized] private ILayoutContainer _previousContainer;
        [field: NonSerialized] private int _previousContainerIndex = -1;
        private object _toolTip;

        internal LayoutContent()
        {
        }

        /// <summary>
        /// Event fired when the content is closed (i.e. removed definitely from the layout)
        /// </summary>
        public event EventHandler Closed;

        /// <summary>
        /// Event fired when the content is about to be closed (i.e. removed definitely from the layout)
        /// </summary>
        /// <remarks>Please note that LayoutAnchorable also can be hidden. Usually user hide anchorables when click the 'X' button. To completely close 
        /// an anchorable the user should click the 'Close' menu item from the context menu. When an LayoutAnchorable is hidden its visibility changes to false and
        /// IsHidden property is set to true.
        /// Handle the Hiding event for the LayoutAnchorable to cancel the hide operation.</remarks>
        public event EventHandler<CancelEventArgs> Closing;

        public event EventHandler IsActiveChanged;
        public event EventHandler IsSelectedChanged;

        public int CompareTo(LayoutContent other)
        {
            var contentAsComparable = Content as IComparable;
            if (contentAsComparable != null)
            {
                return contentAsComparable.CompareTo(other.Content);
            }

            return _comparer.Compare(Title, other.Title);
        }

        public double FloatingHeight
        {
            get => _floatingHeight;
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
            get => _floatingLeft;
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
            get => _floatingTop;
            set
            {
                if (_floatingTop == value)
                    return;
                RaisePropertyChanging("FloatingTop");
                _floatingTop = value;
                RaisePropertyChanged("FloatingTop");
            }
        }

        public double FloatingWidth
        {
            get => _floatingWidth;
            set
            {
                if (_floatingWidth == value)
                    return;
                RaisePropertyChanging("FloatingWidth");
                _floatingWidth = value;
                RaisePropertyChanged("FloatingWidth");
            }
        }

        public bool IsMaximized
        {
            get => _isMaximized;
            set
            {
                if (_isMaximized == value)
                    return;
                RaisePropertyChanging("IsMaximized");
                _isMaximized = value;
                RaisePropertyChanged("IsMaximized");
            }
        }

        [XmlIgnore]
        ILayoutContainer ILayoutPreviousContainer.PreviousContainer
        {
            get { return _previousContainer; }
            set
            {
                if (_previousContainer != value)
                {
                    _previousContainer = value;
                    RaisePropertyChanged("PreviousContainer");

                    var paneSerializable = _previousContainer as ILayoutPaneSerializable;
                    if (paneSerializable != null &&
                        paneSerializable.Id == null)
                        paneSerializable.Id = Guid.NewGuid().ToString();
                }
            }
        }

        [XmlIgnore]
        string ILayoutPreviousContainer.PreviousContainerId { get; set; }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public virtual void ReadXml(XmlReader reader)
        {
            if (reader.MoveToAttribute("Title"))
                Title = reader.Value;
            //if (reader.MoveToAttribute("IconSource"))
            //    IconSource = new Uri(reader.Value, UriKind.RelativeOrAbsolute);

            if (reader.MoveToAttribute("IsSelected"))
                IsSelected = bool.Parse(reader.Value);
            if (reader.MoveToAttribute("ContentId"))
                ContentId = reader.Value;
            if (reader.MoveToAttribute("IsLastFocusedDocument"))
                IsLastFocusedDocument = bool.Parse(reader.Value);
            if (reader.MoveToAttribute("PreviousContainerId"))
                PreviousContainerId = reader.Value;
            if (reader.MoveToAttribute("PreviousContainerIndex"))
                PreviousContainerIndex = int.Parse(reader.Value);

            if (reader.MoveToAttribute("FloatingLeft"))
                FloatingLeft = double.Parse(reader.Value, CultureInfo.InvariantCulture);
            if (reader.MoveToAttribute("FloatingTop"))
                FloatingTop = double.Parse(reader.Value, CultureInfo.InvariantCulture);
            if (reader.MoveToAttribute("FloatingWidth"))
                FloatingWidth = double.Parse(reader.Value, CultureInfo.InvariantCulture);
            if (reader.MoveToAttribute("FloatingHeight"))
                FloatingHeight = double.Parse(reader.Value, CultureInfo.InvariantCulture);
            if (reader.MoveToAttribute("IsMaximized"))
                IsMaximized = bool.Parse(reader.Value);
            if (reader.MoveToAttribute("CanClose"))
                CanClose = bool.Parse(reader.Value);
            if (reader.MoveToAttribute("CanFloat"))
                CanFloat = bool.Parse(reader.Value);
            if (reader.MoveToAttribute("LastActivationTimeStamp"))
                LastActivationTimeStamp = DateTime.Parse(reader.Value, CultureInfo.InvariantCulture);

            reader.Read();
        }

        public virtual void WriteXml(XmlWriter writer)
        {
            if (!string.IsNullOrWhiteSpace(Title))
                writer.WriteAttributeString("Title", Title);

            //if (IconSource != null)
            //    writer.WriteAttributeString("IconSource", IconSource.ToString());

            if (IsSelected)
                writer.WriteAttributeString("IsSelected", IsSelected.ToString());

            if (IsLastFocusedDocument)
                writer.WriteAttributeString("IsLastFocusedDocument", IsLastFocusedDocument.ToString());

            if (!string.IsNullOrWhiteSpace(ContentId))
                writer.WriteAttributeString("ContentId", ContentId);


            var s = ToolTip as string;
            if (!string.IsNullOrWhiteSpace(s))
                writer.WriteAttributeString("ToolTip", s);

            if (FloatingLeft != 0.0)
                writer.WriteAttributeString("FloatingLeft", FloatingLeft.ToString(CultureInfo.InvariantCulture));
            if (FloatingTop != 0.0)
                writer.WriteAttributeString("FloatingTop", FloatingTop.ToString(CultureInfo.InvariantCulture));
            if (FloatingWidth != 0.0)
                writer.WriteAttributeString("FloatingWidth", FloatingWidth.ToString(CultureInfo.InvariantCulture));
            if (FloatingHeight != 0.0)
                writer.WriteAttributeString("FloatingHeight", FloatingHeight.ToString(CultureInfo.InvariantCulture));

            if (IsMaximized)
                writer.WriteAttributeString("IsMaximized", IsMaximized.ToString());
            if (!CanClose)
                writer.WriteAttributeString("CanClose", CanClose.ToString());
            if (!CanFloat)
                writer.WriteAttributeString("CanFloat", CanFloat.ToString());

            if (LastActivationTimeStamp != null)
                writer.WriteAttributeString("LastActivationTimeStamp",
                    LastActivationTimeStamp.Value.ToString(CultureInfo.InvariantCulture));

            var paneSerializable = _previousContainer as ILayoutPaneSerializable;
            if (paneSerializable == null)
                return;
            writer.WriteAttributeString("PreviousContainerId", paneSerializable.Id);
            writer.WriteAttributeString("PreviousContainerIndex", _previousContainerIndex.ToString());
        }

        public bool IsFloating => this.FindParent<LayoutFloatingWindow>() != null;

        public bool CanClose
        {
            get => _canClose;
            set
            {
                if (_canClose == value)
                    return;
                _canClose = value;
                RaisePropertyChanged("CanClose");
            }
        }

        public bool CanFloat
        {
            get => _canFloat;
            set
            {
                if (_canFloat == value)
                    return;
                _canFloat = value;
                RaisePropertyChanged("CanFloat");
            }
        }

        [XmlIgnore]
        public object Content
        {
            get => _content;
            set
            {
                if (_content == value)
                    return;
                RaisePropertyChanging("Content");
                _content = value;
                RaisePropertyChanged("Content");
            }
        }

        public string ContentId
        {
            get
            {
                if (_contentId != null)
                    return _contentId;
                var contentAsControl = _content as FrameworkElement;
                if (string.IsNullOrWhiteSpace(contentAsControl?.Name))
                    return _contentId;
                return contentAsControl.Name;
            }
            set
            {
                if (_contentId == value)
                    return;
                _contentId = value;
                RaisePropertyChanged("ContentId");
            }
        }

        public ImageSource IconSource
        {
            get => _iconSource;
            set
            {
                if (Equals(_iconSource, value))
                    return;
                _iconSource = value;
                RaisePropertyChanged("IconSource");
            }
        }

        [XmlIgnore]
        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (_isActive == value)
                    return;
                RaisePropertyChanging("IsActive");
                bool oldValue = _isActive;

                _isActive = value;

                var root = Root;
                if (root != null && _isActive)
                    root.ActiveContent = this;

                if (_isActive)
                    IsSelected = true;

                OnIsActiveChanged(oldValue, value);
                RaisePropertyChanged("IsActive");
            }
        }

        [XmlIgnore]
        public bool IsLastFocusedDocument
        {
            get { return _isLastFocusedDocument; }
            internal set
            {
                if (_isLastFocusedDocument != value)
                {
                    RaisePropertyChanging("IsLastFocusedDocument");
                    _isLastFocusedDocument = value;
                    RaisePropertyChanged("IsLastFocusedDocument");
                }
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected == value)
                    return;
                bool oldValue = _isSelected;
                RaisePropertyChanging("IsSelected");
                _isSelected = value;
                var parentSelector = (Parent as ILayoutContentSelector);
                if (parentSelector != null)
                    parentSelector.SelectedContentIndex = _isSelected ? parentSelector.IndexOf(this) : -1;
                OnIsSelectedChanged(oldValue, value);
                RaisePropertyChanged("IsSelected");
            }
        }

        public DateTime? LastActivationTimeStamp
        {
            get => _lastActivationTimeStamp;
            set
            {
                if (_lastActivationTimeStamp == value)
                    return;
                _lastActivationTimeStamp = value;
                RaisePropertyChanged("LastActivationTimeStamp");
            }
        }

        [XmlIgnore]
        public int PreviousContainerIndex
        {
            get => _previousContainerIndex;
            set
            {
                if (_previousContainerIndex == value)
                    return;
                _previousContainerIndex = value;
                RaisePropertyChanged("PreviousContainerIndex");
            }
        }

        public string Title
        {
            get => (string) GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public object ToolTip
        {
            get => _toolTip;
            set
            {
                if (_toolTip == value)
                    return;
                _toolTip = value;
                RaisePropertyChanged("ToolTip");
            }
        }

        protected ILayoutContainer PreviousContainer
        {
            get => ((ILayoutPreviousContainer) this).PreviousContainer;
            set => ((ILayoutPreviousContainer) this).PreviousContainer = value;
        }

        protected string PreviousContainerId
        {
            get => ((ILayoutPreviousContainer) this).PreviousContainerId;
            set => ((ILayoutPreviousContainer) this).PreviousContainerId = value;
        }

        /// <summary>
        /// Close the content
        /// </summary>
        /// <remarks>Please note that usually the anchorable is only hidden (not closed). By default when user click the X button it only hides the content.</remarks>
        public void Close()
        {
            var root = Root;
            var parentAsContainer = Parent;
            parentAsContainer.RemoveChild(this);
            root?.CollectGarbage();

            OnClosed();
        }

        /// <summary>
        /// Re-dock the content to its previous container
        /// </summary>
        public void Dock()
        {
            if (PreviousContainer != null)
            {
                var currentContainer = Parent;
                var currentContainerIndex = (currentContainer as ILayoutGroup)?.IndexOfChild(this) ?? -1;
                var previousContainerAsLayoutGroup = PreviousContainer as ILayoutGroup;

                if (previousContainerAsLayoutGroup != null &&
                    PreviousContainerIndex < previousContainerAsLayoutGroup.ChildrenCount)
                    previousContainerAsLayoutGroup.InsertChildAt(PreviousContainerIndex, this);
                else
                    previousContainerAsLayoutGroup?.InsertChildAt(previousContainerAsLayoutGroup.ChildrenCount, this);

                if (currentContainerIndex > -1)
                {
                    PreviousContainer = currentContainer;
                    PreviousContainerIndex = currentContainerIndex;
                }
                else
                {
                    PreviousContainer = null;
                    PreviousContainerIndex = 0;
                }

                IsSelected = true;
                IsActive = true;
            }
            else
                InternalDock();
            ShowMainWindow();
            Root.CollectGarbage();
        }

        private void ShowMainWindow()
        {
            var owner = Application.Current.MainWindow;
            if (owner != null)
            {
                if (owner.WindowState == WindowState.Minimized)
                    SystemCommands.RestoreWindow(owner);
                owner.Activate();
                owner.Topmost = true;
                owner.Topmost = false;
                owner.Focus();
            }
        }

        /// <summary>
        /// Dock the content as document
        /// </summary>
        public void DockAsDocument()
        {
            var root = Root as LayoutRoot;
            if (root == null)
                throw new InvalidOperationException();
            if (Parent is LayoutDocumentPane)
                return;

            if (PreviousContainer is LayoutDocumentPane)
            {
                Dock();
                return;
            }

            LayoutDocumentPane newParentPane;
            if (root.LastFocusedDocument != null)
            {
                newParentPane = root.LastFocusedDocument.Parent as LayoutDocumentPane ??
                                root.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
            }
            else
            {
                newParentPane = root.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
            }

            if (newParentPane != null)
            {
                newParentPane.Children.Add(this);
                root.CollectGarbage();
            }

            IsSelected = true;
            IsActive = true;
        }

        /// <summary>
        /// Float the content in a popup window
        /// </summary>
        public void Float()
        {
            if (PreviousContainer?.FindParent<LayoutFloatingWindow>() != null)
            {
                var currentContainer = Parent as ILayoutPane;
                var layoutGroup = currentContainer as ILayoutGroup;
                if (layoutGroup != null)
                {
                    var currentContainerIndex = layoutGroup.IndexOfChild(this);
                    var previousContainerAsLayoutGroup = PreviousContainer as ILayoutGroup;

                    if (previousContainerAsLayoutGroup != null && PreviousContainerIndex <
                        previousContainerAsLayoutGroup.ChildrenCount)
                        previousContainerAsLayoutGroup.InsertChildAt(PreviousContainerIndex, this);
                    else
                        previousContainerAsLayoutGroup?.InsertChildAt(previousContainerAsLayoutGroup.ChildrenCount, this);

                    PreviousContainer = currentContainer;
                    PreviousContainerIndex = currentContainerIndex;
                }

                IsSelected = true;
                IsActive = true;

                Root.CollectGarbage();
            }
            else
            {
                Root.Manager.StartDraggingFloatingWindowForContent(this, false);

                IsSelected = true;
                IsActive = true;
            }
        }

        protected virtual void InternalDock()
        {
        }

        protected virtual void OnClosed()
        {
            Closed?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnClosing(CancelEventArgs args)
        {
            Closing?.Invoke(this, args);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the IsActive property.
        /// </summary>
        protected virtual void OnIsActiveChanged(bool oldValue, bool newValue)
        {
            if (newValue)
                LastActivationTimeStamp = DateTime.Now;

            IsActiveChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the IsSelected property.
        /// </summary>
        protected virtual void OnIsSelectedChanged(bool oldValue, bool newValue)
        {
            IsSelectedChanged?.Invoke(this, EventArgs.Empty);
        }

        protected override void OnParentChanged(ILayoutContainer oldValue, ILayoutContainer newValue)
        {
            if (IsSelected && Parent is ILayoutContentSelector)
            {
                var parentSelector = ((ILayoutContentSelector) Parent);
                parentSelector.SelectedContentIndex = parentSelector.IndexOf(this);
            }

            //var root = Root;
            //if (root != null && _isActive)
            //    root.ActiveContent = this;

            base.OnParentChanged(oldValue, newValue);
        }

        protected override void OnParentChanging(ILayoutContainer oldValue, ILayoutContainer newValue)
        {
            if (oldValue != null)
                IsSelected = false;

            //if (root != null && _isActive && newValue == null)
            //    root.ActiveContent = null;

            base.OnParentChanging(oldValue, newValue);
        }

        protected virtual void SetXmlAttributeValue(string name, string valueString)
        {
            switch (name)
            {
                case "Title":
                    Title = valueString;
                    break;
                case "ToolTip":
                    ToolTip = valueString;
                    break;
                case "IsSelected":
                    IsSelected = bool.Parse(valueString);
                    break;
                case "CanClose":
                    CanClose = bool.Parse(valueString);
                    break;
                case "CanFloat":
                    CanFloat = bool.Parse(valueString);
                    break;
                case "IsLastFocusedDocument":
                    IsLastFocusedDocument = bool.Parse(valueString);
                    break;
                case "PreviousContainerId":
                    PreviousContainerId = valueString;
                    break;
                case "PreviousContainerIndex":
                    PreviousContainerIndex = int.Parse(valueString);
                    break;
                case "ContentId":
                    ContentId = valueString;
                    break;
                case "FloatingLeft":
                    FloatingLeft = XmlConvert.ToDouble(valueString);
                    break;
                case "FloatingTop":
                    FloatingTop = XmlConvert.ToDouble(valueString);
                    break;
                case "FloatingWidth":
                    FloatingWidth = XmlConvert.ToDouble(valueString);
                    break;
                case "FloatingHeight":
                    FloatingHeight = XmlConvert.ToDouble(valueString);
                    break;
                case "LastActivationTimeStamp":
                    LastActivationTimeStamp = DateTime.Parse(valueString, CultureInfo.InvariantCulture);
                    break;
            }
        }

        /// <summary>
        /// Test if the content can be closed
        /// </summary>
        /// <returns></returns>
        internal bool TestCanClose()
        {
            CancelEventArgs args = new CancelEventArgs();
            OnClosing(args);
            return !args.Cancel;
        }

        internal void XmlDeserialize(XmlReader xmlReader)
        {
            bool isEmptyElement = xmlReader.IsEmptyElement;
            if (xmlReader.HasAttributes)
            {
                while (xmlReader.MoveToNextAttribute())
                {
                    string name = xmlReader.Name;
                    string valueString = xmlReader.Value;

                    MethodInfo methodInfo = GetType()
                        .GetMethod("SetXmlAttributeValue", BindingFlags.Instance | BindingFlags.NonPublic);
                    methodInfo.Invoke(this, new object[]
                    {
                        name, valueString
                    });
                }
            }

            if (!isEmptyElement)
            {
                while (xmlReader.Read())
                {
                    if (xmlReader.NodeType == XmlNodeType.EndElement)
                    {
                        break;
                    }
                }
            }
        }

        private static object CoerceTitleValue(DependencyObject obj, object value)
        {
            var lc = (LayoutContent) obj;
            if (((string) value) != lc.Title)
            {
                lc.RaisePropertyChanging(TitleProperty.Name);
            }
            return value;
        }

        private static void OnTitlePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ((LayoutContent) obj).RaisePropertyChanged(TitleProperty.Name);
        }

        private bool _isEnabled = true;
        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    RaisePropertyChanged("IsEnabled");
                }
            }
        }
    }
}