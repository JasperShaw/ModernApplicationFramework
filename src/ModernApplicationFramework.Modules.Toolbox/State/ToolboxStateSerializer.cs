using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Xml;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Services;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;
using ModernApplicationFramework.Modules.Toolbox.Items;
using ModernApplicationFramework.Utilities.Xml;

namespace ModernApplicationFramework.Modules.Toolbox.State
{
    [Export(typeof(IToolboxStateSerializer))]
    public class ToolboxStateSerializer : LayoutSerializer<IToolboxNode>, IToolboxStateSerializer
    {
        protected override string RootNode => "ToolboxLayoutState";

        protected override Stream ValidationScheme => Stream.Null;

        protected override void ClearCurrentLayout()
        {
        }

        protected override void Deserialize(ref XmlNode xmlRootNode)
        {
        }

        protected override void EnsureInitialized()
        {
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
            //foreach (var type in _stateCache.GetKeys())
            //{
            //    SerializeTargetType(type, ref lastChild, ref xmlDocument);   
            //}
            //SerializeDefaultCategory(ref lastChild, ref xmlDocument);
        }

        private void SerializeTargetType(Type type, ref XmlNode parentElement, ref XmlDocument document)
        {
            //var targetElement = document.CreateElement("TargetType", null, new KeyValuePair<string, string>("Type", type.FullName));

            //var categories = _stateCache.GetState(type);
            //if (categories == null || !categories.Any())
            //    return;

            ////We do not need to save default-only types
            //if (categories.Count == 1 && ToolboxItemCategory.IsDefaultCategory(categories.First()))
            //    return;

            //foreach (var category in categories)
            //{
            //    SerializeCategory(category, ref targetElement, ref document);
            //}
            //parentElement.AppendChild(targetElement);
        }


        private void SerializeCategory(IToolboxCategory category, ref XmlElement parentElement, ref XmlDocument document)
        {
            XmlElement xCategory;
            if (ToolboxItemCategory.IsDefaultCategory(category))
            {
                xCategory = document.CreateElement(null, "DefaultCategory", null);
            }
            else
            {
                xCategory = document.CreateElement("Category", null,
                    new KeyValuePair<string, string>("Id", category.Id.ToString("B")));
                foreach (var toolboxItem in category.Items)
                {
                    SerializeItem(toolboxItem, ref xCategory, ref document);
                }
            }
            parentElement.AppendChild(xCategory);
        }

        private void SerializeItem(IToolboxItem item, ref XmlElement parentElement, ref XmlDocument document)
        {
            var xItem = document.CreateElement("Item", null,
                new KeyValuePair<string, string>("Id", item.Id.ToString("B")));
            parentElement.AppendChild(xItem);
        }

        private void SerializeDefaultCategory(ref XmlNode parentElement, ref XmlDocument document)
        {
            var category = ToolboxItemCategory.DefaultCategory;
            var xCategory = document.CreateElement("DefaultCategoryDefinition", null, new KeyValuePair<string, string>("Id", category.Id.ToString("B")));
            foreach (var toolboxItem in category.Items)
            {
                SerializeItem(toolboxItem, ref xCategory, ref document);
            }
            parentElement.AppendChild(xCategory);
        }
    }
}
