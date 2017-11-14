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
        private IEnumerable<MenuBarDefinition> _allMenuBars;
        private readonly List<CommandBarDefinitionBase> _allDefinitions = new List<CommandBarDefinitionBase>();
        private ICommandBarDefinitionHost _definitionHost;
        private IEnumerable<CommandBarItemDefinition> _allCommandBarItems;


        private void EnsureInitialized()
        {
            _definitionHost = IoC.Get<ICommandBarDefinitionHost>();
        }

        public void Serialize()
        {
            EnsureInitialized();

            var xmlDocument = CreateDocument();


            SerializeMenuBars(xmlDocument.LastChild);
            SerializeToolBars(xmlDocument.LastChild);
            SerializeContextMenus(xmlDocument.LastChild);


            xmlDocument.Save(@"C:\Test\CommandBar.xml");
        }

        public void Deserialize()
        {
            EnsureInitialized();

            _allMenuBars = IoC.GetAll<MenuBarDefinition>();
            _allCommandBarItems = IoC.GetAll<CommandBarItemDefinition>();

            _allDefinitions.AddRange(_allMenuBars);
            _allDefinitions.AddRange(_allCommandBarItems);


            ClearCurrentLayout();
            DeserializeMenuBars();



        }

        private void DeserializeMenuBars()
        {
            var menuBarHost = IoC.Get<IMenuHostViewModel>();
            menuBarHost.TopLevelDefinitions.Clear();
            menuBarHost.Build();


            var document = new XmlDocument();
            document.Load(@"C:\Test\CommandBar.xml");

            var menuBarsNode = document.DocumentElement?.SelectSingleNode("/CommandBarDefinitions/MenuBars");

            if (menuBarsNode == null || !menuBarsNode.HasChildNodes)
                return;

            foreach (XmlNode menuBarNode in menuBarsNode.ChildNodes)
            {
                var menuBar = FindCommandBarDefinitionById<MenuBarDefinition>(menuBarNode.Attributes["Id"].Value);
                BuildCommandBar(menuBarNode, menuBar);
                menuBarHost.TopLevelDefinitions.Add(menuBar);
            }
            menuBarHost.Build();
        }

        private void BuildCommandBar(XmlNode parentNode, CommandBarDefinitionBase parentDefinition)
        {
            parentDefinition.ContainedGroups.Clear();
            foreach (XmlNode childNode in parentNode.ChildNodes)
            {
                if (!uint.TryParse(childNode.Attributes["SortOrder"].Value, out var sortOrder))
                    throw new ArgumentException("Could not parse sort order");

                if (childNode.Name == "GroupDefinition")
                {
                    var group = CreateGroup(parentDefinition, sortOrder);
                    group.ContainedGroups.Clear();
                    BuildCommandBar(childNode, group);
                    _definitionHost.ItemGroupDefinitions.Add(group);
                }
                else if (childNode.Name == "MenuDefinition")
                {
                    if (!Guid.TryParse(childNode.Attributes["Id"].Value, out var guid))
                        throw new ArgumentException("Could not parse id");
                    MenuDefinition menu = null;
                    if (guid == Guid.Empty)
                    {
                        //TODO
                    }
                    else
                    {
                        menu = FindCommandBarDefinitionById<MenuDefinition>(guid);
                    }
                    if (menu == null)
                        throw new ArgumentException("MenuDefinition not found");

                    menu.ContainedGroups.Clear();
                    if (parentDefinition is CommandBarGroupDefinition parentGroup)
                        menu.Group = parentGroup;
                    _definitionHost.ItemDefinitions.Add(menu);
                }
                else if (childNode.Name == "ItemDefinition")
                {
                    if (!Guid.TryParse(childNode.Attributes["Id"].Value, out var guid))
                        throw new ArgumentException("Could not parse id");

                    CommandBarItemDefinition item = null;
                    if (guid == Guid.Empty)
                    {

                    }
                    else
                        item = FindCommandBarDefinitionById<CommandBarItemDefinition>(guid);

                    if (item == null)
                        continue; //TODO throw
                    item.ContainedGroups.Clear();
                    if (parentDefinition is CommandBarGroupDefinition parentGroup)
                        item.Group = parentGroup;
                    _definitionHost.ItemDefinitions.Add(item);
                }
            }
        }

        private CommandBarGroupDefinition CreateGroup(CommandBarDefinitionBase parent, uint sortOrder)
        {
            return new CommandBarGroupDefinition(parent, sortOrder);
        }


        private T FindCommandBarDefinitionById<T>(string id) where T : CommandBarDefinitionBase
        {
            if (!Guid.TryParse(id, out var guid))
                throw new ArgumentException("Could not parse id");
            return FindCommandBarDefinitionById<T>(guid);
        }

        private T FindCommandBarDefinitionById<T>(Guid guid) where T : CommandBarDefinitionBase
        {
            var definition = _allDefinitions.FirstOrDefault(x => x.Id.Equals(guid));
            if (definition == null)
                throw new ArgumentException("Definition not found");
            return (T) definition;
        }



        private void ClearCurrentLayout()
        {
            _definitionHost.ItemGroupDefinitions.Clear();
            _definitionHost.ItemDefinitions.Clear();
            _definitionHost.ExcludedItemDefinitions.Clear();
        }



        #region Serialize

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
                    new KeyValuePair<string, string>("Id", menuBar.Id.ToString("B")),
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
                    new KeyValuePair<string, string>("SortOrder", group.SortOrder.ToString()),
                    new KeyValuePair<string, string>("ItemCount", group.Items.Count.ToString()));


                foreach (var groupItem in group.Items.OrderBy(x => x.SortOrder))
                {
                    XmlElement itemElement = null;
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

            itemElement.SetAttribute("Id", commandBarDefinition.Id.ToString("B"));
            itemElement.SetAttribute("SortOrder", commandBarDefinition.SortOrder.ToString());
            itemElement.SetAttribute("Flags", commandBarDefinition.Flags.AllFlags.ToString());


            return itemElement;
        }

        #endregion


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
        void Deserialize();
    }
}