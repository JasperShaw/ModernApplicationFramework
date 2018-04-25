using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Caliburn.Micro;
using GongSolutions.Wpf.DragDrop;
using ModernApplicationFramework.Extended.Demo.Modules.FilterDesigner.Design;
using ModernApplicationFramework.Extended.Demo.Modules.FilterDesigner.Util;
using ModernApplicationFramework.Extended.Demo.Modules.FilterDesigner.ViewModels.Elements;
using ModernApplicationFramework.Extended.Layout;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Modules.Inspector;
using ModernApplicationFramework.Modules.Toolbox;
using ImageSource = ModernApplicationFramework.Extended.Demo.Modules.FilterDesigner.ViewModels.Elements.ImageSource;

namespace ModernApplicationFramework.Extended.Demo.Modules.FilterDesigner.ViewModels
{
    [Export(typeof(GraphViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class GraphViewModel : KeyBindingLayoutItem, IDropTarget
    {
        private readonly IInspectorTool _inspectorTool;
        private readonly BindableCollection<ElementViewModel> _elements;
        private readonly BindableCollection<ConnectionViewModel> _connections;

        public IObservableCollection<ElementViewModel> Elements => _elements;

        public IObservableCollection<ConnectionViewModel> Connections => _connections;

        public override GestureScope GestureScope => GestureScopes.GlobalGestureScope;

        public IEnumerable<ElementViewModel> SelectedElements
        {
            get { return _elements.Where(x => x.IsSelected); }
        }

        [ImportingConstructor]
        public GraphViewModel(IInspectorTool inspectorTool)
        {
            DisplayName = "[New Graph]";

            _elements = new BindableCollection<ElementViewModel>();
            _connections = new BindableCollection<ConnectionViewModel>();

            _inspectorTool = inspectorTool;

            var element1 = AddElement<ImageSource>(100, 50);
            element1.Bitmap = BitmapUtility.CreateFromBytes(DesignTimeImages.Desert);

            var element2 = AddElement<ColorInput>(100, 300);
            element2.Color = Colors.Green;

            var element3 = AddElement<Add>(400, 250);


            Connections.Add(new ConnectionViewModel(
                element1.OutputConnector,
                element3.InputConnectors[0]));

            Connections.Add(new ConnectionViewModel(
                element2.OutputConnector,
                element3.InputConnectors[1]));

            element1.IsSelected = true;
        }

        public TElement AddElement<TElement>(double x, double y)
            where TElement : ElementViewModel, new()
        {
            var element = new TElement { X = x, Y = y };
            _elements.Add(element);
            return element;
        }

        public ConnectionViewModel OnConnectionDragStarted(ConnectorViewModel sourceConnector, Point currentDragPoint)
        {
            if (!(sourceConnector is OutputConnectorViewModel))
                return null;

            var connection = new ConnectionViewModel((OutputConnectorViewModel)sourceConnector)
            {
                ToPosition = currentDragPoint
            };

            Connections.Add(connection);

            return connection;
        }

        public void OnConnectionDragging(Point currentDragPoint, ConnectionViewModel connection)
        {
            // If current drag point is close to an input connector, show its snapped position.
            var nearbyConnector = FindNearbyInputConnector(currentDragPoint);
            connection.ToPosition = nearbyConnector?.Position ?? currentDragPoint;
        }

        public void OnConnectionDragCompleted(Point currentDragPoint, ConnectionViewModel newConnection, ConnectorViewModel sourceConnector)
        {
            var nearbyConnector = FindNearbyInputConnector(currentDragPoint);

            if (nearbyConnector == null || sourceConnector.Element == nearbyConnector.Element)
            {
                Connections.Remove(newConnection);
                return;
            }

            var existingConnection = nearbyConnector.Connection;
            if (existingConnection != null)
                Connections.Remove(existingConnection);

            newConnection.To = nearbyConnector;
        }

        public void DeleteElement(ElementViewModel element)
        {
            Connections.RemoveRange(element.AttachedConnections);
            Elements.Remove(element);
        }

        public void DeleteSelectedElements()
        {
            Elements.Where(x => x.IsSelected)
                .ToList()
                .ForEach(DeleteElement);
        }

        public void OnSelectionChanged()
        {
            var selectedElements = SelectedElements.ToList();

            if (selectedElements.Count == 1)
                _inspectorTool.SelectedObject = new InspectableObjectBuilder()
                    .WithObjectProperties(selectedElements[0], x => true)
                    .ToInspectableObject();
            else
                _inspectorTool.SelectedObject = null;
        }

        private InputConnectorViewModel FindNearbyInputConnector(Point mousePosition)
        {
            return Elements.SelectMany(x => x.InputConnectors)
                .FirstOrDefault(x => AreClose(x.Position, mousePosition, 10));
        }

        private static bool AreClose(Point point1, Point point2, double distance)
        {
            return (point1 - point2).Length < distance;
        }

        public void DragOver(IDropInfo dropInfo)
        {
            if (dropInfo.Data is ToolboxItemEx)
                dropInfo.Effects = DragDropEffects.All;
        }

        public void Drop(IDropInfo dropInfo)
        {
            if (!(dropInfo.Data is ToolboxItemEx toolboxItem))
                return;
            var mousePosition = dropInfo.DropPosition;
            var element = (ElementViewModel)Activator.CreateInstance(toolboxItem.TargetType);
            element.X = mousePosition.X;
            element.Y = mousePosition.Y;
            Elements.Add(element);
        }
    }
}
