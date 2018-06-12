using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml;
using ModernApplicationFramework.Basics.Services;
using ModernApplicationFramework.Core.Utilities;
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

        protected override string RootNode => "ToolboxLayoutState";

        protected override Stream ValidationScheme => Stream.Null;

        [ImportingConstructor]
        public ToolboxStateSerializer(IToolboxService service, IToolboxStateProvider provider)
        {
            _service = service;
            _provider = provider;
        }

        protected override void ClearCurrentLayout()
        {
            _service.GetToolboxItemSource().ForEach(x => x.Items.Clear());
            _service.StoreAndApplyLayout(new List<IToolboxCategory>());
        }

        protected override void Deserialize(ref XmlNode xmlRootNode)
        {
            var state = DeserializeNode(in xmlRootNode, guid => _service.GetCategoryById(guid),
                node =>
                {
                    node.TryGetValueResult<string>("Name", out var name);
                    return string.IsNullOrEmpty(name) ? null : new ToolboxItemCategory(Guid.Empty, name, true);
                },
                (category, node) =>
                {
                    var items = DeserializeNode(in node, guid => _service.GetItemById(guid), itemNode =>
                    {
                        itemNode.TryGetValueResult<string>("Name", out var name);

                        if (string.IsNullOrEmpty(name))
                            return null;

                        var data = ParseData(in itemNode);
                        var typeList = GetCompatibleTypeList(in itemNode);

                        if (data == null || typeList == null)
                            return null;

                        return new ToolboxItem(name, data, typeList);
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
            var xCategory = CreateElement(ref document, "Category", category);

            foreach (var toolboxItem in category.Items)
            {
                SerializeItem(toolboxItem, ref xCategory, ref document);
            }
            parentElement.AppendChild(xCategory);
        }

        private static void SerializeItem(IToolboxItem item, ref XmlNode parentElement, ref XmlDocument document)
        {
            if (!item.Serializable)
                return;

            var doc = document;
            var xItem = CreateElement(ref document, "Item", item, element =>
            {
                if (!item.IsCustom)
                    return;

                foreach (var type in item.CompatibleTypes.Memebers)
                {
                    var xType = doc.CreateElement("CompatibleType");
                    xType.SetAttribute("Type", type.FullName);
                    element.AppendChild(xType);
                }

                var data = SerializeItemData(ref doc, item.Data);
                if (data != null)
                    element.AppendChild(data);
            });

            parentElement.AppendChild(xItem);
        }

        private static XmlNode SerializeItemData(ref XmlDocument doc, IDataObject data)
        {
            var xData = doc.CreateElement("Data");

            var format = data.GetFormats().FirstOrDefault();
            if (format == null)
                return null;


            var rawString = data.GetData(format)?.ToString() ?? string.Empty;
            var value = Convert.ToBase64String(GZip.Compress(Encoding.UTF8.GetBytes(rawString)));

            xData.SetAttribute("Format", format);
            xData.SetAttribute("Value", value);
            return xData;
        }

        private static XmlNode CreateElement(ref XmlDocument document, string name, IToolboxNode node,
            Action<XmlNode> fillElementFunc = null)
        {
            var element = document.CreateElement(name);

            element.SetAttribute("Id", node.Id.ToString("B"));

            if (node.IsNameModified || node.IsCustom)
                element.SetAttribute("Name", node.Name);

            fillElementFunc?.Invoke(element);

            return element;
        }



        private static IEnumerable<T> DeserializeNode<T>(in XmlNode node, Func<Guid, T> findNodeFunc,
    Func<XmlNode, T> guidEmptyFunc = null, Action<T, XmlNode> prefillFunc = null) where T : IToolboxNode
        {
            var list = new List<T>();

            foreach (XmlNode xNodeItem in node.ChildNodes)
            {
                var guid = xNodeItem.GetAttributeValue<Guid>("Id");
                xNodeItem.TryGetValueResult<string>("Name", out var name);

                if (guid == Guid.Empty && guidEmptyFunc == null || findNodeFunc == null)
                    continue;

                var nodeItem = guid == Guid.Empty ? guidEmptyFunc(xNodeItem) : findNodeFunc(guid);
                if (nodeItem == null)
                    continue;

                prefillFunc?.Invoke(nodeItem, xNodeItem);

                if (!string.IsNullOrEmpty(name))
                    nodeItem.Name = name;

                list.Add(nodeItem);
            }
            return list;
        }

        private static IDataObject ParseData(in XmlNode node)
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
            return new DataObject(format, data);
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
    }
}
