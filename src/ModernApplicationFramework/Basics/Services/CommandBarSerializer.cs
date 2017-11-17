using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Controls;
using System.Xml;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.ContextMenu;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Basics.Definitions.Toolbar;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.ViewModels;
using ModernApplicationFramework.Utilities.Xml;
using static System.Globalization.CultureInfo;

namespace ModernApplicationFramework.Basics.Services
{
    [Export(typeof(ICommandBarSerializer))]
    public class CommandBarSerializer : ICommandBarSerializer
    {
        //private IEnumerable<MenuBarDefinition> _allMenuBars;
        private readonly List<CommandBarDefinitionBase> _allDefinitions = new List<CommandBarDefinitionBase>();
        private ICommandBarDefinitionHost _definitionHost;
        private IEnumerable<CommandBarItemDefinition> _allCommandBarItems;
        private IEnumerable<CommandDefinitionBase> _allCommandDefintions;


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

            var allMenuBars = IoC.GetAll<MenuBarDefinition>();
            var allToolBars = IoC.GetAll<ToolbarDefinition>();
            _allCommandBarItems = IoC.GetAll<CommandBarItemDefinition>();
            _allCommandDefintions = IoC.GetAll<CommandDefinitionBase>();

            _allDefinitions.AddRange(allMenuBars);
            _allDefinitions.AddRange(allToolBars);
            _allDefinitions.AddRange(_allCommandBarItems);



            ClearCurrentLayout();

            var document = new XmlDocument();
            document.Load(@"C:\Test\CommandBar.xml");

            DeserializeCommandBar<MenuBarDefinition, IMenuHostViewModel>(document, "/CommandBarDefinitions/MenuBars");
            DeserializeCommandBar<ToolbarDefinition, IToolBarHostViewModel>(document, "/CommandBarDefinitions/Toolbars");
        }

        #region Deserialize

        private void DeserializeCommandBar<T, TV>(XmlDocument document,  string path) where T : CommandBarDefinitionBase
            where TV : ICommandBarHost
        {

            var commandBarHost = IoC.Get<TV>();
            commandBarHost.TopLevelDefinitions.Clear();
            commandBarHost.Build();

            var node = document.DocumentElement?.SelectSingleNode(path);

            if (node == null || !node.HasChildNodes)
                return;

            foreach (XmlNode commandBarNode in node.ChildNodes)
            {

                var guid = commandBarNode.GetAttributeValue<Guid>("Id");
                commandBarNode.TryGetValueResult<string>("Text", out var text);
                commandBarNode.TryGetValueResult<uint>("SortOrder", out var sortOrder);
                commandBarNode.TryGetValueResult<bool>("IsVisible", out var visible);
                commandBarNode.TryGetValueResult<int>("Position", out var position);
                CommandBarDefinitionBase commandBar = null;
                if (guid == Guid.Empty)
                {
                    if (typeof(T) == typeof(ToolbarDefinition))
                    {
                        commandBar = new ToolbarDefinition(guid, text, sortOrder, visible,(Dock) position, true, true);
                    }
                    else if (typeof(T) == typeof(ContextMenuDefinition))
                    {
                        
                    }
                }
                else
                    commandBar = FindCommandBarDefinitionById<T>(guid);

                if (commandBar is ToolbarDefinition toolbar)
                {
                    toolbar.Position = (Dock)position;
                    toolbar.IsVisible = visible;
                }
                BuildCommandBar(commandBarNode, commandBar);
                commandBarHost.TopLevelDefinitions.Add(commandBar);
            }
            commandBarHost.Build();
        }

        private void BuildCommandBar(XmlNode parentNode, CommandBarDefinitionBase parentDefinition)
        {
            parentDefinition.ContainedGroups.Clear();
            foreach (XmlNode childNode in parentNode.ChildNodes)
            {
                if (childNode.Name == "GroupDefinition")
                    CreateCommandBarGroup(parentDefinition, childNode);
                else if (childNode.Name == "MenuDefinition")
                    CreateCommandBarMenu(parentDefinition, childNode);
                else if (childNode.Name == "ItemDefinition")
                    CreateCommandBarItem(parentDefinition, childNode);
                else if (childNode.Name == "MenuControllerDefinition")
                    CreateCommandBarMenuControllerItem(parentDefinition, childNode);
                else if (childNode.Name == "ComboBoxDefinition")
                    CreateCommandBarComboBoxItem(parentDefinition, childNode);
                else if (childNode.Name == "SplitButtonDefinition")
                    CreateCommandSplitButtonItem(parentDefinition, childNode);
            }
        }

        private void CreateCommandSplitButtonItem(CommandBarDefinitionBase parentDefinition, XmlNode childNode)
        {
            var guid = childNode.GetAttributeValue<Guid>("Id");
            var sortOrder = childNode.GetAttributeValue<uint>("SortOrder");
            childNode.TryGetValueResult<string>("Text", out var text);

            CommandBarSplitItemDefinition item;
            if (guid == Guid.Empty)
            {
                var commandId = childNode.GetAttributeValue<Guid>("Command");
                if (commandId == Guid.Empty)
                    throw new NotSupportedException("CommandId cannot be 'Guid.Empty'");
                var command = _allCommandDefintions.FirstOrDefault(x => x.Id.Equals(commandId));
                if (command == null)
                    throw new ArgumentNullException("Command was not found");
                item = new CommandBarSplitItemDefinition(guid, text, sortOrder, null, command, true, false, true);
            }
            else
                item = FindCommandBarDefinitionById<CommandBarSplitItemDefinition>(guid);

            if (item == null)
                throw new ArgumentNullException("CommandBarSplitItemDefinition not found");

            AssignGroup(item, parentDefinition);
            SetFlags(item, childNode);
            item.SortOrder = sortOrder;
            if (text != null)
                item.Text = text;
            _definitionHost.ItemDefinitions.Add(item);
        }

        private void CreateCommandBarGroup(CommandBarDefinitionBase parentDefinition, XmlNode childNode)
        {
            var sortOrder = childNode.GetAttributeValue<uint>("SortOrder");
            var group = CreateGroup(parentDefinition, sortOrder);
            group.ContainedGroups.Clear();
            BuildCommandBar(childNode, group);
            group.SortOrder = sortOrder;
            _definitionHost.ItemGroupDefinitions.Add(group);
        }

        private void CreateCommandBarMenu(CommandBarDefinitionBase parentDefinition, XmlNode childNode)
        {
            var guid = childNode.GetAttributeValue<Guid>("Id");
            var sortOrder = childNode.GetAttributeValue<uint>("SortOrder");
            childNode.TryGetValueResult<string>("Text", out var text);
            MenuDefinition menu;
            if (guid == Guid.Empty)
            {
                if (!(parentDefinition is CommandBarGroupDefinition group))
                    throw new ArgumentException("Parent must be a group");
                menu = new MenuDefinition(guid, group, sortOrder, text, true);
            }
            else
                menu = FindCommandBarDefinitionById<MenuDefinition>(guid);

            if (menu == null)
                throw new ArgumentNullException("MenuDefinition not found");

            AssignGroup(menu, parentDefinition);
            SetFlags(menu, childNode);
            menu.SortOrder = sortOrder;
            if (text != null)
                menu.Text = text;
            BuildCommandBar(childNode, menu);

            _definitionHost.ItemDefinitions.Add(menu);
        }

        private void CreateCommandBarItem(CommandBarDefinitionBase parentDefinition, XmlNode childNode)
        {
            var guid = childNode.GetAttributeValue<Guid>("Id");
            var sortOrder = childNode.GetAttributeValue<uint>("SortOrder");
            childNode.TryGetValueResult<string>("Text", out var text);

            CommandBarItemDefinition item;
            if (guid == Guid.Empty)
            {
                var commandId = childNode.GetAttributeValue<Guid>("Command");
                if (commandId == Guid.Empty)
                    throw new NotSupportedException("CommandId cannot be 'Guid.Empty'");
                var command = _allCommandDefintions.FirstOrDefault(x => x.Id.Equals(commandId));
                if (command == null)
                    throw new ArgumentNullException("Command was not found");
                item = new CommandBarCommandItemDefinition(guid, sortOrder, command);
            }
            else
                item = FindCommandBarDefinitionById<CommandBarItemDefinition>(guid);

            if (item == null)
                throw new ArgumentNullException("ItemDefinition not found");

            AssignGroup(item, parentDefinition);
            SetFlags(item, childNode);
            item.SortOrder = sortOrder;
            if (text != null)
                item.Text = text;
            _definitionHost.ItemDefinitions.Add(item);
        }

        private void CreateCommandBarMenuControllerItem(CommandBarDefinitionBase parentDefinition, XmlNode childNode)
        {
            var guid = childNode.GetAttributeValue<Guid>("Id");
            var sortOrder = childNode.GetAttributeValue<uint>("SortOrder");
            if (guid == Guid.Empty)
                throw new NotSupportedException("Menu Controller can not be custom");

            var menuController = FindCommandBarDefinitionById<CommandBarMenuControllerDefinition>(guid);

            if (menuController == null)
                throw new ArgumentNullException("CommandBarMenuControllerDefinition not found");

            AssignGroup(menuController, parentDefinition);
            SetFlags(menuController, childNode);
            menuController.SortOrder = sortOrder;
            _definitionHost.ItemDefinitions.Add(menuController);
        }

        private void CreateCommandBarComboBoxItem(CommandBarDefinitionBase parentDefinition, XmlNode childNode)
        {
            var guid = childNode.GetAttributeValue<Guid>("Id");
            var sortOrder = childNode.GetAttributeValue<uint>("SortOrder");
            var vFlags = childNode.GetAttributeValue<int>("VisualFlags");
            var editable = childNode.GetAttributeValue<bool>("IsEditable");
            var dropDownWidth = childNode.GetAttributeValue<double>("DropDownWidth");
            childNode.TryGetValueResult<string>("Text", out var text);

            CommandBarComboItemDefinition comboboxItem;
            if (guid == Guid.Empty)
            {
                var commandId = childNode.GetAttributeValue<Guid>("Command");
                if (commandId == Guid.Empty)
                    throw new NotSupportedException("CommandId cannot be 'Guid.Empty'");
                var command = _allCommandDefintions.FirstOrDefault(x => x.Id.Equals(commandId));
                if (command == null)
                    throw new ArgumentNullException("Command was not found");
                comboboxItem =
                    new CommandBarComboItemDefinition(guid, text, sortOrder, null, command, true, false, true);
            }
            else
                comboboxItem = FindCommandBarDefinitionById<CommandBarComboItemDefinition>(guid);

            if (comboboxItem == null)
                throw new ArgumentNullException("CommandBarComboItemDefinition not found");

            AssignGroup(comboboxItem, parentDefinition);
            SetFlags(comboboxItem, childNode);
            comboboxItem.SortOrder = sortOrder;
            comboboxItem.VisualSource.Flags.EnableStyleFlags((CommandBarFlags) vFlags);
            comboboxItem.VisualSource.IsEditable = editable;
            comboboxItem.VisualSource.DropDownWidth = dropDownWidth;
            if (text != null)
                comboboxItem.Text = text;
            _definitionHost.ItemDefinitions.Add(comboboxItem);
        }

        #endregion

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

            var document = parentElement.OwnerDocument;

            var toolBarsElement = parentElement.OwnerDocument.CreateElement(string.Empty, "Toolbars", string.Empty);


            var toolBars = IoC.Get<IToolBarHostViewModel>().GetMenuHeaderItemDefinitions()
                .Where(x => x is ToolbarDefinition).Cast<ToolbarDefinition>();

            foreach (var toolBar in toolBars)
            {
                var toolBarElement = document.CreateElement("ToolBar", string.Empty,
                    new KeyValuePair<string, string>("Id", toolBar.Id.ToString("B")),
                    new KeyValuePair<string, string>("Position", ((int)toolBar.Position).ToString()),
                    new KeyValuePair<string, string>("IsVisible", toolBar.IsVisible.ToString()),
                    new KeyValuePair<string, string>("SortOrder", toolBar.SortOrder.ToString()));
                ExplodeGroups(toolBar, toolBarElement, document);

                if (toolBar.IsCustom)
                    toolBarElement.SetAttribute("Text", toolBar.Text);

                toolBarsElement.AppendChild(toolBarElement);
            }


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
                    switch (groupItem)
                    {
                        case MenuDefinition menuDefinition:
                            itemElement = CreateElement(CreationType.MenuDefinition, document, menuDefinition);
                            break;
                        case CommandBarMenuControllerDefinition menuController:
                            itemElement = CreateElement(CreationType.MenuControllerDefinition, document, menuController);
                            break;
                        case CommandBarComboItemDefinition comboItemDefinition:
                            itemElement = CreateElement(CreationType.ComboBoxItemDefinition, document, comboItemDefinition);
                            break;
                        case CommandBarSplitItemDefinition splitItemDefinition:
                            itemElement = CreateElement(CreationType.SplitButtonItemDefinition, document, splitItemDefinition);
                            break;
                        case CommandBarItemDefinition commandItem:
                            itemElement = CreateElement(CreationType.ItemDefinition, document, commandItem);
                            break;
                    }
                    ExplodeGroups(groupItem, itemElement, document);
                    groupElement.AppendChild(itemElement);
                }


                parentXmlElement.AppendChild(groupElement);
            }
        }

        private static XmlDocument CreateDocument()
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
                    itemElement = CreateMenuDefinitionElement(document);
                    break;
                case CreationType.MenuControllerDefinition:
                    itemElement = CreateMenuControllerElement(document, commandBarDefinition);
                    break;
                case CreationType.ItemDefinition:
                    itemElement = CreateCommandBarItemElement(document, commandBarDefinition);
                    break;
                case CreationType.ComboBoxItemDefinition:
                    itemElement = CreateComboBoxItemElement(document, commandBarDefinition);
                    break;
                case CreationType.SplitButtonItemDefinition:
                    itemElement = CreateSplitButtonItemElement(document, commandBarDefinition);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            itemElement.SetAttribute("Id", commandBarDefinition.Id.ToString("B"));
            itemElement.SetAttribute("SortOrder", commandBarDefinition.SortOrder.ToString());
            itemElement.SetAttribute("Flags", commandBarDefinition.Flags.AllFlags.ToString());

            if (commandBarDefinition.IsTextModified || commandBarDefinition.IsCustom)

                itemElement.SetAttribute("Text", commandBarDefinition.Text);


            return itemElement;
        }

        private XmlElement CreateSplitButtonItemElement(XmlDocument document, CommandBarItemDefinition commandBarDefinition)
        {
            return document.CreateElement("SplitButtonDefinition", string.Empty,
                new KeyValuePair<string, string>("Command", commandBarDefinition.CommandDefinition.Id.ToString("B")));
        }

        private static XmlElement CreateMenuDefinitionElement(XmlDocument document)
        {
            return document.CreateElement(string.Empty, "MenuDefinition", string.Empty);
        }

        private static XmlElement CreateMenuControllerElement(XmlDocument document, CommandBarItemDefinition definition)
        {
            var menuController = definition as CommandBarMenuControllerDefinition;
            var element = document.CreateElement("MenuControllerDefinition", string.Empty,
                new KeyValuePair<string, string>("AnchroItem",
                    menuController.AnchorItem?.CommandDefinition?.Id.ToString("B")),
                new KeyValuePair<string, string>("IsVisible", definition.IsVisible.ToString()));

            if (!(definition.CommandDefinition is CommandMenuControllerDefinition controllerDefinition))
                return element;
            foreach (var item in controllerDefinition.Items)
            {
                var innerItemElement = document.CreateElement("ItemDefinition", string.Empty,
                    new KeyValuePair<string, string>("Command",
                        item.CommandDefinition.Id.ToString("B")));
                element.AppendChild(innerItemElement);
            }
            return element;
        }

        private static XmlElement CreateCommandBarItemElement(XmlDocument document, CommandBarItemDefinition definition)
        {
            return document.CreateElement("ItemDefinition", string.Empty,
                new KeyValuePair<string, string>("Command",
                    definition.CommandDefinition.Id.ToString("B")));
        }

        private static XmlElement CreateComboBoxItemElement(XmlDocument document, CommandBarItemDefinition definition)
        {
            var comboDefinition = definition as CommandBarComboItemDefinition;

            var element =  document.CreateElement("ComboBoxDefinition", string.Empty,
                new KeyValuePair<string, string>("Command", comboDefinition.CommandDefinition.Id.ToString("B")),
                new KeyValuePair<string, string>("VisualFlags", comboDefinition.VisualSource.Flags.AllFlags.ToString()),
                new KeyValuePair<string, string>("IsEditable", comboDefinition.VisualSource.IsEditable.ToString()),
                new KeyValuePair<string, string>("DropDownWidth", comboDefinition.VisualSource.DropDownWidth.ToString(InvariantCulture)));


            return element;
        }

        #endregion

        private T FindCommandBarDefinitionById<T>(Guid guid) where T : CommandBarDefinitionBase
        {
            var definition = _allDefinitions.FirstOrDefault(x => x.Id.Equals(guid));
            if (definition == null)
                throw new ArgumentException("Definition not found");
            return (T)definition;
        }

        private void ClearCurrentLayout()
        {
            _definitionHost.ItemGroupDefinitions.Clear();
            _definitionHost.ItemDefinitions.Clear();
            _definitionHost.ExcludedItemDefinitions.Clear();
        }

        private static CommandBarGroupDefinition CreateGroup(CommandBarDefinitionBase parent, uint sortOrder)
        {
            return new CommandBarGroupDefinition(parent, sortOrder);
        }

        private static void AssignGroup(CommandBarItemDefinition item, CommandBarDefinitionBase parentDefinition)
        {
            item.ContainedGroups.Clear();
            if (parentDefinition is CommandBarGroupDefinition parentGroup)
                item.Group = parentGroup;
        }

        private static void SetFlags(CommandBarDefinitionBase item, XmlNode node)
        {
            var flags = node.GetAttributeValue<int>("Flags");
            item.Flags.EnableStyleFlags((CommandBarFlags)flags);
        }


        private enum CreationType
        {
            MenuDefinition,
            MenuControllerDefinition,
            ComboBoxItemDefinition,
            SplitButtonItemDefinition,
            ItemDefinition
        }
    }



    public interface ICommandBarSerializer
    {
        void Serialize();
        void Deserialize();
    }
}