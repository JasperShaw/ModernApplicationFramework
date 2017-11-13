using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Xml;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.ViewModels;
using ModernApplicationFramework.Utilities.Xml;

namespace ModernApplicationFramework.Basics.Services
{
    [Export(typeof(ICommandBarSerializer))]
    public class CommandBarSerializer : ICommandBarSerializer
    {
        public void Serialize()
        {
            var xmlDocument = CreateDocument();


            SerializeMenuBars(xmlDocument.LastChild);
            SerializeToolBars(xmlDocument.LastChild);
            SerializeContextMenus(xmlDocument.LastChild);


            xmlDocument.Save(@"C:\Test\CommandBar.xml");
        }

        private void SerializeContextMenus(XmlNode parentElement)
        {
            if (parentElement.OwnerDocument == null)
                throw new InvalidOperationException();
            var contextMenusElement =
                parentElement.OwnerDocument.CreateElement(string.Empty, "ContextMenus", string.Empty);

            parentElement.AppendChild(contextMenusElement);
        }

        private void SerializeToolBars(XmlNode parentElement)
        {
            if (parentElement.OwnerDocument == null)
                throw new InvalidOperationException();
            var toolBarsElement = parentElement.OwnerDocument.CreateElement(string.Empty, "Toolbars", string.Empty);

            parentElement.AppendChild(toolBarsElement);
        }

        private void SerializeMenuBars(XmlNode parentElement)
        {
            if (parentElement.OwnerDocument == null)
                throw new InvalidOperationException();
            var document = parentElement.OwnerDocument;
            var menuBarsElement = document.CreateElement(string.Empty, "MenuBars", string.Empty);


            var menuBars = IoC.Get<IMenuHostViewModel>().GetMenuHeaderItemDefinitions()
                .Where(x => x is MenuBarDefinition).Cast<MenuBarDefinition>();

            foreach (var menuBar in menuBars)
            {
                var menuBarElement = document.CreateElement("MenuBar", string.Empty,
                    new KeyValuePair<string, string>("Name", menuBar.InternalName),
                    new KeyValuePair<string, string>("SortOrder", menuBar.SortOrder.ToString()));


                ExplodeGroups(menuBar, menuBarElement, document);


                menuBarsElement.AppendChild(menuBarElement);
            }
            parentElement.AppendChild(menuBarsElement);
        }

        private void ExplodeGroups(CommandBarDefinitionBase definition, XmlNode parentXmlElement, XmlDocument document)
        {
            if (definition.ContainedGroups == null || definition.ContainedGroups.Count == 0)
                return;

            foreach (var group in definition.ContainedGroups.OrderBy(x => x.SortOrder))
            {
                var groupElement = document.CreateElement("GroupDefinition", string.Empty,
                    new KeyValuePair<string, string>("ItemCount", group.Items.Count.ToString()));


                foreach (var groupItem in group.Items.OrderBy(x => x.SortOrder))
                {
                    XmlElement itemElement;
                    if (groupItem is MenuDefinition menuDefinition)
                        itemElement = document.CreateElement("MenuDefinition", string.Empty,
                            new KeyValuePair<string, string>("SortOrder", menuDefinition.SortOrder.ToString()),
                            new KeyValuePair<string, string>("IsVisible", menuDefinition.IsVisible.ToString()));
                    else if (groupItem is CommandBarMenuControllerDefinition menuController)
                    {
                        itemElement = document.CreateElement("MenuControllerDefinition", string.Empty,
                            new KeyValuePair<string, string>("AnchroItem",
                                menuController.AnchorItem?.CommandDefinition?.Id.ToString()),
                            new KeyValuePair<string, string>("IsVisible", menuController.IsVisible.ToString()));

                        if (menuController.CommandDefinition is CommandMenuControllerDefinition controllerDefinition)
                            foreach (var item in controllerDefinition.Items)
                            {
                                var innerItemElement =  document.CreateElement("ItemDefinition", string.Empty,
                                    new KeyValuePair<string, string>("Command",
                                        item.CommandDefinition.Id.ToString("B")));
                                itemElement.AppendChild(innerItemElement);
                            }
                    }
                    else if (groupItem is CommandBarItemDefinition commandItem)
                        itemElement = document.CreateElement("ItemDefinition", string.Empty,
                            new KeyValuePair<string, string>("IsVisible", commandItem.IsVisible.ToString()),
                            new KeyValuePair<string, string>("SortOrder", commandItem.SortOrder.ToString()),
                            new KeyValuePair<string, string>("Command",
                                commandItem.CommandDefinition.Id.ToString("B")));
                    else
                        continue;
                    ExplodeGroups(groupItem, itemElement, document);
                    groupElement.AppendChild(itemElement);
                }


                parentXmlElement.AppendChild(groupElement);
            }
        }


        private XmlDocument CreateDocument()
        {
            var xmlDocument = new XmlDocument();
            var xmlDeclaration = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);
            var root = xmlDocument.DocumentElement;
            xmlDocument.InsertBefore(xmlDeclaration, root);

            var rootElement = xmlDocument.CreateElement(string.Empty, "CommandBarDefinitions", string.Empty);
            xmlDocument.AppendChild(rootElement);

            return xmlDocument;
        }

        private void CreateElement()
        {
        }
    }

    public interface ICommandBarSerializer
    {
        void Serialize();
    }
}