using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Xml;
using ModernApplicationFramework.Basics.Services;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;

namespace ModernApplicationFramework.Modules.Toolbox.State
{
    [Export(typeof(IToolboxStateSerializer))]
    internal class ToolboxStateSerializer : LayoutSerializer<IToolboxNode>, IToolboxStateSerializer
    {
        private readonly IToolbox _toolbox;
        private readonly IToolboxService _service;
        private readonly IToolboxStateProvider _provider;

        protected override string RootNode => "ToolboxLayoutState";

        protected override Stream ValidationScheme => Stream.Null;

        [ImportingConstructor]
        public ToolboxStateSerializer(IToolbox toolbox, IToolboxService service, IToolboxStateProvider provider)
        {
            _toolbox = toolbox;
            _service = service;
            _provider = provider;
        }

        protected override void ClearCurrentLayout()
        {
        }

        protected override void Deserialize(ref XmlNode xmlRootNode)
        {
        }

        protected override void EnsureInitialized()
        {
            _service.StoreItemsSource(_toolbox.Categories);
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

        private void SerializeCategory(IToolboxCategory category, ref XmlNode parentElement, ref XmlDocument document)
        {
            var xCategory = CreateElement(ref document, "Category", category);

            foreach (var toolboxItem in category.Items)
            {
                SerializeItem(toolboxItem, ref xCategory, ref document);
            }
            parentElement.AppendChild(xCategory);
        }

        private void SerializeItem(IToolboxItem item, ref XmlNode parentElement, ref XmlDocument document)
        {
            if (!item.Serializable)
                return;

            var doc = document;
            var xItem = CreateElement(ref document, "Item", item, element =>
            {
                foreach (var type in item.CompatibleTypes.Memebers)
                {
                    var xType = doc.CreateElement("CompatibleType");
                    xType.SetAttribute("Type", type.FullName);
                    element.AppendChild(xType);
                }              
            });

            parentElement.AppendChild(xItem);
        }


        private static XmlNode CreateElement(ref XmlDocument document, string name, IToolboxNode node,
            Action<XmlNode> fillElementFunc = null)
        {
            var element = document.CreateElement(name);

            element.SetAttribute("Id", node.Id.ToString("B"));

            if (node.IsNameModified || node.IsCustom)
                element.SetAttribute("Text", node.Name);

            fillElementFunc?.Invoke(element);

            return element;
        }
    }
}
