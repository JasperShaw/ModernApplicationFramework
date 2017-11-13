﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Xml;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Menu;
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
                    XmlElement itemElement =null;
                    if (groupItem is MenuDefinition menuDefinition)
                        itemElement = CreateElement(CreationType.MenuDefinition, document, menuDefinition);
                    else if (groupItem is CommandBarMenuControllerDefinition menuController)
                        itemElement = CreateElement(CreationType.MenuControllerDefinition, document, menuController);
                    else if (groupItem is CommandBarItemDefinition commandItem)
                        itemElement = CreateElement(CreationType.ItemDefinition, document, commandItem);
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

        private XmlElement CreateElement(CreationType type, XmlDocument document, CommandBarItemDefinition commandBarDefinition)
        {
            XmlElement itemElement;

            switch (type)
            {
                case CreationType.MenuDefinition:
                    itemElement = document.CreateElement(string.Empty, "MenuDefinition", string.Empty);
                    break;
                case CreationType.MenuControllerDefinition:

                    var menuController = commandBarDefinition as CommandBarMenuControllerDefinition;

                    itemElement = document.CreateElement("MenuControllerDefinition", string.Empty,
                        new KeyValuePair<string, string>("AnchroItem",
                            menuController.AnchorItem?.CommandDefinition?.Id.ToString()),
                        new KeyValuePair<string, string>("IsVisible", commandBarDefinition.IsVisible.ToString()));

                    if (commandBarDefinition.CommandDefinition is CommandMenuControllerDefinition controllerDefinition)
                        foreach (var item in controllerDefinition.Items)
                        {
                            var innerItemElement = document.CreateElement("ItemDefinition", string.Empty,
                                new KeyValuePair<string, string>("Command",
                                    item.CommandDefinition.Id.ToString("B")));
                            itemElement.AppendChild(innerItemElement);
                        }
                    break;
                case CreationType.ItemDefinition:
                    itemElement = document.CreateElement("ItemDefinition", string.Empty,
                        new KeyValuePair<string, string>("Command",
                            commandBarDefinition.CommandDefinition.Id.ToString("B")));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }


            itemElement.SetAttribute("SortOrder", commandBarDefinition.SortOrder.ToString());
            itemElement.SetAttribute("Flags", commandBarDefinition.Flags.AllFlags.ToString());


            return itemElement;
        }

        private enum CreationType
        {
            MenuDefinition,
            MenuControllerDefinition,
            ItemDefinition
        }
    }



    public interface ICommandBarSerializer
    {
        void Serialize();
    }
}