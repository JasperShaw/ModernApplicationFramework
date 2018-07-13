using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Text;
using System.Windows;
using System.Xml;
using ModernApplicationFramework.Basics.Services;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Imaging;
using ModernApplicationFramework.Imaging.Interop;
using ModernApplicationFramework.Modules.Toolbox.Extensions;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;
using ModernApplicationFramework.Modules.Toolbox.Items;
using ModernApplicationFramework.Utilities;
using ModernApplicationFramework.Utilities.Xml;

namespace ModernApplicationFramework.Modules.Toolbox.State
{
    [Export(typeof(IToolboxStateSerializer))]
    internal class ToolboxStateSerializer : LayoutSerializer<IToolboxNode>, IToolboxStateSerializer
    {
        private readonly IToolboxService _service;
        private readonly IToolboxStateProvider _provider;
        private readonly ToolboxItemDefinitionHost _definitionHost;

        protected override string RootNode => "ToolboxLayoutState";

        protected override Stream ValidationScheme => Properties.Resources.ToolboxStateScheme.ToStream();

        [ImportingConstructor]
        public ToolboxStateSerializer(IToolboxService service, IToolboxStateProvider provider, ToolboxItemDefinitionHost definitionHost)
        {
            _service = service;
            _provider = provider;
            _definitionHost = definitionHost;
        }

        protected override void ClearCurrentLayout()
        {
            _service.GetToolboxItemSource().ForEach(x => x.Items.Clear());
            _service.StoreAndApplyLayout(new List<IToolboxCategory>());
        }


        private T CreateNode<T>(Guid itemId, Guid definitionId) where T : class, IToolboxNode
        {
            if (typeof(T) == typeof(IToolboxItem))
            {
                var definition = _definitionHost.GetItemDefinitionById(definitionId);
                return new ToolboxItem(itemId, null, definition) as T;
            }
            if (typeof(T) == typeof(IToolboxCategory))
            {
                var definition = _definitionHost.GetCategoryDefinitionById(definitionId);
                return new ToolboxCategory(itemId, definition) as T;
            }
            return default;
        }


        protected override void Deserialize(ref XmlNode xmlRootNode)
        {
            var state = DeserializeNode(in xmlRootNode, CreateNode<IToolboxCategory>,
                node =>
                {
                    node.TryGetValueResult<string>("Name", out var name);
                    return string.IsNullOrEmpty(name) ? null : new ToolboxCategory(name);
                },
                (category, node) =>
                {
                    var items = DeserializeNode(in node, CreateNode<IToolboxItem>, itemNode =>
                    {
                        itemNode.TryGetValueResult<string>("Name", out var name);

                        if (string.IsNullOrEmpty(name))
                            return null;

                        var data = ParseData(in itemNode);
                        var typeList = GetCompatibleTypeList(in itemNode);

                        if (data == null || typeList == null)
                            return null;

                        if (data.Format == DataFormats.Text)
                            return ToolboxItem.CreateTextItem(data);

                        var image = GetImageMoniker(in itemNode);
                        var dataSource = new ToolboxItemDefinition(name, data, typeList, image);
                        return new ToolboxItem(dataSource);
                    });
                    foreach (var item in items)
                        category.Items.Add(item);
                });
            _service.StoreAndApplyLayout(state);
        }

        protected override void EnsureInitialized()
        {
            _service.StoreCurrentLayout();
        }

        protected override XmlNode GetBackupNode(in XmlDocument backup, IToolboxNode item)
        {
            return null;
        }

        protected override XmlNode GetCurrentNode(in XmlDocument currentLayout, IToolboxNode item)
        {
            return null;
        }

        protected override void HandleBackupNodeNull(IToolboxNode item)
        {
        }

        protected override void PrepareDeserialize()
        {
        }

        protected override void Serialize(ref XmlDocument xmlDocument)
        {
            if (xmlDocument == null)
                return;
            var lastChild = xmlDocument.LastChild;

            foreach (var category in _provider.ItemsSource)
            {
                SerializeCategory(category, ref lastChild, ref xmlDocument);
            }
        }

        private static void SerializeCategory(IToolboxCategory category, ref XmlNode parentElement, ref XmlDocument document)
        {
            var xCategory = CreateCategoryNode(ref document, category);

            foreach (var toolboxItem in category.Items)
            {
                SerializeItem(toolboxItem, ref xCategory, ref document);
            }
            parentElement.AppendChild(xCategory);
        }

        private static void SerializeItem(IToolboxItem item, ref XmlNode parentElement, ref XmlDocument document)
        {
            if (!item.DataSource.Serializable)
                return;

            var doc = document;
            var xItem = CreateItemNode(ref document, "Item", item, element =>
            {
                if (!item.IsCustom || item.DataSource.Id != Guid.Empty)
                    return;

                foreach (var type in item.DataSource.CompatibleTypes.Memebers)
                {
                    var xType = doc.CreateElement("CompatibleType");
                    xType.SetAttribute("Type", type.FullName);
                    element.AppendChild(xType);
                }
                var data = SerializeItemData(ref doc, item.DataSource.Data);
                if (data != null)
                    element.AppendChild(data);

                if (item.DataSource.Data.Format == DataFormats.Text)
                    return;
                var image = SerializeImageMoniker(ref doc, item.DataSource.ImageMoniker);
                if (image != null)
                    element.AppendChild(image);
            });

            parentElement.AppendChild(xItem);
        }

        private static XmlNode SerializeImageMoniker(ref XmlDocument doc, ImageMoniker moniker)
        {
            var xData = doc.CreateElement("Image");
            if (moniker.Equals(ImageLibrary.EmptyMoniker))
                return null;

            xData.SetAttribute("CatalogGuid", moniker.CatalogGuid.ToString("B"));
            xData.SetAttribute("Id", moniker.Id.ToString());
            return xData;
        }

        private static XmlNode SerializeItemData(ref XmlDocument doc, ToolboxItemData data)
        {
            var xData = doc.CreateElement("Data");

            var format = data.Format;
            if (format == null)
                return null;


            var rawString = data.Data?.ToString() ?? string.Empty;
            var value = Convert.ToBase64String(GZip.Compress(Encoding.UTF8.GetBytes(rawString)));

            xData.SetAttribute("Format", format);
            xData.SetAttribute("Value", value);
            return xData;
        }



        private static XmlNode CreateCategoryNode(ref XmlDocument document, IToolboxCategory node)
        {
            var element = document.CreateElement("Category");
            element.SetAttribute("Id", node.Id.ToString("B"));
            element.SetAttribute("DefinitionId", node.DataSource.Id.ToString("B"));
            if (node.IsNameModified || node.IsCustom)
                element.SetAttribute("Name", node.Name);

            return element;
        }




        private static XmlNode CreateItemNode(ref XmlDocument document, string name, IToolboxItem node,
            Action<XmlNode> fillElementFunc = null)
        {
            var element = document.CreateElement(name);

            element.SetAttribute("Id", node.Id.ToString("B"));
            element.SetAttribute("DefinitionId", node.DataSource.Id.ToString("B"));

            if (node.IsNameModified || node.IsCustom)
                element.SetAttribute("Name", node.Name);

            fillElementFunc?.Invoke(element);

            return element;
        }



        private static IEnumerable<T> DeserializeNode<T>(in XmlNode node, Func<Guid, Guid, T> findNodeFunc,
    Func<XmlNode, T> guidEmptyFunc = null, Action<T, XmlNode> prefillFunc = null) where T : IToolboxNode
        {
            var list = new List<T>();

            foreach (XmlNode xNodeItem in node.ChildNodes)
            {
                var iGuid = xNodeItem.GetAttributeValue<Guid>("Id");
                var dGuid = xNodeItem.GetAttributeValue<Guid>("DefinitionId");
                xNodeItem.TryGetValueResult<string>("Name", out var name);

                if (iGuid == Guid.Empty && guidEmptyFunc == null || findNodeFunc == null)
                    continue;

                var nodeItem = dGuid == Guid.Empty ? guidEmptyFunc(xNodeItem) : findNodeFunc(iGuid, dGuid);
                if (nodeItem == null)
                    continue;

                prefillFunc?.Invoke(nodeItem, xNodeItem);

                if (!string.IsNullOrEmpty(name))
                    nodeItem.Name = name;

                list.Add(nodeItem);
            }
            return list;
        }

        private static ToolboxItemData ParseData(in XmlNode node)
        {
            var dataNode = node.SelectSingleNode("Data");

            if (dataNode == null)
                return null;

            var format = dataNode.GetAttributeValue<string>("Format");
            var value = dataNode.GetAttributeValue<string>("Value");

            if (format == null || value == null)
                return null;


            try
            {
                value = Encoding.UTF8.GetString(GZip.Decompress(Convert.FromBase64String(value)));
            }
            catch
            {
                return null;
            }
            

            object data;
            switch (format)
            {
                case ToolboxItemDataFormats.Type:
                    data = TypeUtilities.GetTypeFromAllLoadedAssemblies(value);
                    if (data == null)
                        return null;
                    break;
                case "Text":
                    data = value;
                    break;
                default:
                    return null;
            }
            return new ToolboxItemData(format, data);
        }

        private static IEnumerable<Type> GetCompatibleTypeList(in XmlNode node)
        {
            var typeList = new List<Type>();
            var compatibleTypes = node.SelectNodes("CompatibleType");
            if (compatibleTypes == null)
                return null;
            foreach (XmlNode type in compatibleTypes)
            {
                var typeString = type.GetAttributeValue<string>("Type");
                var convertedType = TypeUtilities.GetTypeFromAllLoadedAssemblies(typeString);
                typeList.Add(convertedType);
            }
            return typeList;
        }

        private static ImageMoniker GetImageMoniker(in XmlNode node)
         {
            var imageNode = node.SelectSingleNode("Image");
            if (imageNode == null)
                return default;
            var guid = imageNode.GetAttributeValue<Guid>("CatalogGuid");
            var id = imageNode.GetAttributeValue<int>("Id");

            return new ImageMoniker {CatalogGuid = guid, Id = id};
        }
    }
}
