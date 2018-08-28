using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Xml;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.CommandBar.Elements;
using ModernApplicationFramework.Basics.Definitions.ItemDefinitions;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Interfaces.ViewModels;
using ModernApplicationFramework.Utilities.Xml;
using static System.Globalization.CultureInfo;

namespace ModernApplicationFramework.Basics.Services
{

    //TODO: Re-Implement IsVisible and IsCustom for every item type
    [Export(typeof(ICommandBarSerializer))]
    public class CommandBarSerializer : LayoutSerializer<CommandBarDataSource>, ICommandBarSerializer
    {
        private readonly List<CommandBarDataSource> _allDefinitions = new List<CommandBarDataSource>();
        private ICommandBarDefinitionHost _definitionHost;
        private IEnumerable<CommandBarDataSource> _allCommandBarItems;
        private IEnumerable<CommandBarItemDefinition> _allCommandDefintions;

        protected override string RootNode => "CommandBarDefinitions";

        protected override Stream ValidationScheme => Properties.Resources.validation.ToStream();

        protected override void HandleBackupNodeNull(CommandBarDataSource item)
        {
            foreach (var group in item.ContainedGroups.ToList())
                _definitionHost.ItemGroupDefinitions.Remove(group);
            item.ContainedGroups.Clear();
        }

        protected override XmlNode GetBackupNode(in XmlDocument backup, CommandBarDataSource item)
        {
            return backup.SelectSingleNode($"//*[@Id='{item.Id:B}']");
        }

        protected override XmlNode GetCurrentNode(in XmlDocument currentLayout, CommandBarDataSource item)
        {
            return currentLayout.SelectSingleNode($"//*[@Id='{item.Id:B}']");
        }

        protected override void Deserialize(ref XmlNode xmlRootNode)
        {
            DeserializeCommandBar<MenuBarDataSource, IMenuHostViewModel>(xmlRootNode,
                "//MenuBars");
            DeserializeCommandBar<ToolBarDataSource, IToolBarHostViewModel>(xmlRootNode,
                "//Toolbars", delegate (XmlNode node)
                {
                    node.TryGetValueResult<string>("Text", out var text);
                    node.TryGetValueResult<uint>("BandIndex", out var bandIndex);
                    node.TryGetValueResult<bool>("IsVisible", out var visible);
                    node.TryGetValueResult<int>("Position", out var position);
                    return new ToolBarDataSource(Guid.Empty, text, 0, bandIndex, false, (Dock) position);
                }, (definition, node) =>
                {
                    if (!(definition is ToolBarDataSource toolbar))
                        return;
                    node.TryGetValueResult<bool>("IsVisible", out var visible);
                    node.TryGetValueResult<int>("Position", out var position);
                    node.TryGetValueResult<uint>("PlacementSlot", out var placementSlot);
                    toolbar.Position = (Dock)position;
                    toolbar.IsVisible = visible;
                    toolbar.PlacementSlot = placementSlot;
                });
            DeserializeCommandBar<ContextMenuDataSource, IContextMenuHost>(xmlRootNode,
                "//ContextMenus");
        }

        protected override void Serialize(ref XmlDocument xmlDocument)
        {
            SerializeCommandBarRoot<IMenuHostViewModel, MenuBarDataSource>(xmlDocument.LastChild, "MenuBars",
                (document, definition) => document.CreateElement("MenuBar", string.Empty,
                    new KeyValuePair<string, string>("Id", definition.Id.ToString("B"))));

            SerializeCommandBarRoot<IToolBarHostViewModel, ToolBarDataSource>(xmlDocument.LastChild, "Toolbars",
                (document, definition) =>
                {
                    var toolBarElement = document.CreateElement("ToolBar", string.Empty,
                        new KeyValuePair<string, string>("Id", definition.Id.ToString("B")),
                        new KeyValuePair<string, string>("Position", ((int)definition.Position).ToString()),
                        new KeyValuePair<string, string>("IsVisible", definition.IsVisible.ToString()),
                        new KeyValuePair<string, string>("PlacementSlot", definition.PlacementSlot.ToString()),
                        new KeyValuePair<string, string>("BandIndex", definition.BandIndex.ToString()));
                    if (definition.IsCustom)
                        toolBarElement.SetAttribute("Text", definition.Text);
                    return toolBarElement;
                });

            SerializeCommandBarRoot<IContextMenuHost, ContextMenuDataSource>(xmlDocument.LastChild, "ContextMenus",
                (document, definition) => document.CreateElement("ContextMenu", string.Empty,
                    new KeyValuePair<string, string>("Id", definition.Id.ToString("B"))));
        }

        protected override void EnsureInitialized()
        {
            _definitionHost = IoC.Get<ICommandBarDefinitionHost>();
        }

        protected override void PrepareDeserialize()
        {
            //var allMenuBars = IoC.GetAll<MenuBarDataSource>();
            //var allToolBars = IoC.GetAll<ToolBarDataSource>();
            //var allcontextMenus = IoC.GetAll<ContextMenuDataSource>();
            //_allCommandBarItems = IoC.GetAll<CommandBarItemDataSource>();
            //_allCommandDefintions = IoC.GetAll<CommandDefinitionBase>();

            var allMenuBars = IoC.GetAll<MenuBarDataSource>();
            var allToolBars = IoC.GetAll<ToolBarDataSource>();
            var allcontextMenus = IoC.GetAll<ContextMenuDataSource>();
            _allCommandBarItems = _definitionHost.ItemDefinitions;
            _allCommandDefintions = IoC.GetAll<CommandBarItemDefinition>();

            _allDefinitions.AddRange(allMenuBars);
            _allDefinitions.AddRange(allToolBars);
            _allDefinitions.AddRange(allcontextMenus);
            _allDefinitions.AddRange(_allCommandBarItems);
        }

        protected override void ClearCurrentLayout()
        {
            _definitionHost.ItemGroupDefinitions.Clear();
            _definitionHost.ItemDefinitions.Clear();
        }

        #region Deserialize

        private void DeserializeCommandBar<T, TV>(XmlNode rootXmlElement, string path,
            Func<XmlNode, CommandBarDataSource> guidEmptyFunc = null, Action<T, XmlNode> prefillFunc = null)
            where T : CommandBarDataSource
            where TV : ICommandBarHost
        {
            var commandBarHost = IoC.Get<TV>();
            commandBarHost.TopLevelDefinitions.Clear();
            commandBarHost.Build();

            var node = rootXmlElement.SelectSingleNode(path);
            if (node == null || !node.HasChildNodes)
                return;
            foreach (XmlNode commandBarNode in node.ChildNodes)
            {
                var guid = commandBarNode.GetAttributeValue<Guid>("Id");
                if (guid == Guid.Empty && guidEmptyFunc == null)
                    throw new NotSupportedException("Custom CommandBarRootItem not supported");
                var commandBar = guid == Guid.Empty ? guidEmptyFunc(commandBarNode) : FindCommandBarDefinitionById<T>(guid);
                if (commandBar == null)
                    continue;
                prefillFunc?.Invoke((T)commandBar, commandBarNode);
                BuildCommandBar(commandBarNode, commandBar);
                commandBarHost.TopLevelDefinitions.Add(commandBar);
            }
            commandBarHost.Build();
        }

        private void BuildCommandBar(XmlNode parentNode, CommandBarDataSource parentDefinition)
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

        private void CreateCommandSplitButtonItem(CommandBarDataSource parentDefinition, XmlNode childNode)
        {
            var guid = childNode.GetAttributeValue<Guid>("Id");
            var sortOrder = childNode.GetAttributeValue<uint>("SortOrder");
            childNode.TryGetValueResult("IsVisible", out var visible, true);
            childNode.TryGetValueResult<string>("Text", out var text);

            SplitButtonDataSource buttonDataSource;
            if (guid == Guid.Empty)
            {
                var commandId = childNode.GetAttributeValue<Guid>("Command");
                if (commandId == Guid.Empty)
                    throw new NotSupportedException("CommandId cannot be 'Guid.Empty'");
                var command = _allCommandDefintions.FirstOrDefault(x => x.Id.Equals(commandId));
                if (!(command is SplitButtonDefinition splitDefinition))
                    throw new ArgumentNullException(nameof(parentDefinition));
                buttonDataSource = new SplitButtonDataSource(guid, text, sortOrder, null, splitDefinition, false, false);
            }
            else
                buttonDataSource = FindCommandBarDefinitionById<SplitButtonDataSource>(guid);

            if (buttonDataSource == null)
                return;

            AssignGroup(buttonDataSource, parentDefinition);
            SetFlags(buttonDataSource, childNode);
            buttonDataSource.SortOrder = sortOrder;
            buttonDataSource.IsVisible = visible;
            if (text != null)
                buttonDataSource.Text = text;
            _definitionHost.ItemDefinitions.Add(buttonDataSource);
        }

        private void CreateCommandBarGroup(CommandBarDataSource parentDefinition, XmlNode childNode)
        {
            var sortOrder = childNode.GetAttributeValue<uint>("SortOrder");
            var group = CreateGroup(parentDefinition, sortOrder);
            group.ContainedGroups.Clear();
            BuildCommandBar(childNode, group);
            group.SortOrder = sortOrder;
            _definitionHost.ItemGroupDefinitions.Add(group);
        }

        private void CreateCommandBarMenu(CommandBarDataSource parentDefinition, XmlNode childNode)
        {
            var guid = childNode.GetAttributeValue<Guid>("Id");
            var sortOrder = childNode.GetAttributeValue<uint>("SortOrder");
            childNode.TryGetValueResult("IsVisible", out var visible, true);
            childNode.TryGetValueResult<string>("Text", out var text);
            MenuDataSource menu;
            if (guid == Guid.Empty)
            {
                if (!(parentDefinition is CommandBarGroup group))
                    throw new ArgumentException("Parent must be a group");
                menu = new MenuDataSource(guid, text, group, sortOrder, true, CommandBarFlags.CommandFlagNone);
            }
            else
                menu = FindCommandBarDefinitionById<MenuDataSource>(guid);

            if (menu == null)
                return;

            AssignGroup(menu, parentDefinition);
            SetFlags(menu, childNode);
            menu.SortOrder = sortOrder;
            menu.IsVisible = visible;
            if (text != null)
                menu.Text = text;
            BuildCommandBar(childNode, menu);

            _definitionHost.ItemDefinitions.Add(menu);
        }

        private void CreateCommandBarItem(CommandBarDataSource parentDefinition, XmlNode childNode)
        {
            var guid = childNode.GetAttributeValue<Guid>("Id");
            var sortOrder = childNode.GetAttributeValue<uint>("SortOrder");
            childNode.TryGetValueResult<string>("Text", out var text);
            childNode.TryGetValueResult("IsVisible", out var visible, true);

            CommandBarItemDataSource item;
            if (guid == Guid.Empty)
            {
                var commandId = childNode.GetAttributeValue<Guid>("Command");
                if (commandId == Guid.Empty)
                    throw new NotSupportedException("CommandId cannot be 'Guid.Empty'");
                var command = _allCommandDefintions.FirstOrDefault(x => x.Id.Equals(commandId));
                if (!(command is CommandItemDefinitionBase commandDefinition))
                    throw new ArgumentNullException(nameof(parentDefinition));
                item = new CommandBarCommandItem(guid, null, commandDefinition, null, sortOrder).ItemDataSource as ButtonDataSource;
            }
            else
                item = FindCommandBarDefinitionById<CommandBarItemDataSource>(guid);

            if (item == null)
                return;

            AssignGroup(item, parentDefinition);
            SetFlags(item, childNode);
            item.SortOrder = sortOrder;
            item.IsVisible = visible;
            if (text != null)
                item.Text = text;
            _definitionHost.ItemDefinitions.Add(item);
        }

        private void CreateCommandBarMenuControllerItem(CommandBarDataSource parentDefinition, XmlNode childNode)
        {
            var guid = childNode.GetAttributeValue<Guid>("Id");
            var sortOrder = childNode.GetAttributeValue<uint>("SortOrder");
            childNode.TryGetValueResult("IsVisible", out var visible, true);
            if (guid == Guid.Empty)
                throw new NotSupportedException("Menu Controller can not be custom");

            var menuController = FindCommandBarDefinitionById<MenuControllerDataSource>(guid);

            if (menuController == null)
                return;

            AssignGroup(menuController, parentDefinition);
            SetFlags(menuController, childNode);
            menuController.SortOrder = sortOrder;
            menuController.IsVisible = visible;
            _definitionHost.ItemDefinitions.Add(menuController);
        }

        private void CreateCommandBarComboBoxItem(CommandBarDataSource parentDefinition, XmlNode childNode)
        {
            var guid = childNode.GetAttributeValue<Guid>("Id");
            var sortOrder = childNode.GetAttributeValue<uint>("SortOrder");
            var vFlags = childNode.GetAttributeValue<int>("Flags");
            var editable = childNode.GetAttributeValue<bool>("IsEditable");
            var dropDownWidth = childNode.GetAttributeValue<double>("DropDownWidth");
            childNode.TryGetValueResult<string>("Text", out var text);
            childNode.TryGetValueResult("IsVisible", out var visible, true);

            ComboBoxDataSource comboboxItem;
            if (guid == Guid.Empty)
            {
                var commandId = childNode.GetAttributeValue<Guid>("Command");
                if (commandId == Guid.Empty)
                    throw new NotSupportedException("CommandId cannot be 'Guid.Empty'");
                var command = _allCommandDefintions.FirstOrDefault(x => x.Id.Equals(commandId));
                if (!(command is ComboBoxDefinition comboBoxDefinition))
                    throw new ArgumentNullException(nameof(parentDefinition));
                comboboxItem = new CommandBarComboBox(guid, comboBoxDefinition, null, sortOrder).ItemDataSource as ComboBoxDataSource;
            }
            else
                comboboxItem = FindCommandBarDefinitionById<ComboBoxDataSource>(guid);

            if (comboboxItem == null)
                return;

            AssignGroup(comboboxItem, parentDefinition);
            SetFlags(comboboxItem, childNode);
            comboboxItem.SortOrder = sortOrder;
            comboboxItem.Flags.EnableStyleFlags((CommandBarFlags)vFlags);
            comboboxItem.IsEditable = editable;
            comboboxItem.DropDownWidth = dropDownWidth;
            comboboxItem.IsVisible = visible;
            if (text != null)
                comboboxItem.Text = text;
            _definitionHost.ItemDefinitions.Add(comboboxItem);
        }

        #endregion

        #region Serialize

        private static void SerializeCommandBarRoot<T, TV>(XmlNode parentElement, string path, Func<XmlDocument, TV, XmlElement> createElementFunc)
            where T : ICommandBarHost
            where TV : CommandBarDataSource
        {
            if (parentElement.OwnerDocument == null)
                throw new InvalidOperationException();
            var document = parentElement.OwnerDocument;
            var rootElement = parentElement.OwnerDocument.CreateElement(string.Empty, path, string.Empty);

            var host = IoC.Get<T>();

            var commandBarRootItems = host.GetMenuHeaderItemDefinitions()
                .Where(x => x is TV).Cast<TV>();
            foreach (var item in commandBarRootItems)
            {
                host.BuildLogical(item);
                var element = createElementFunc(document, item);
                ExplodeGroups(item, element, document);
                rootElement.AppendChild(element);
            }
            parentElement.AppendChild(rootElement);
        }

        private static void ExplodeGroups(CommandBarDataSource definition, XmlNode parentXmlElement, XmlDocument document)
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
                        case MenuDataSource menuDefinition:
                            itemElement = CreateElement(document, "MenuDefinition", menuDefinition, element =>
                            {
                                if (!menuDefinition.IsVisible)
                                    element.SetAttribute("IsVisible", false.ToString());
                            });
                            break;
                        case MenuControllerDataSource menuController:
                            itemElement = CreateElement(document, "MenuControllerDefinition",
                                menuController,
                                element =>
                                {
                                    element.SetAttribute("AnchroItem",
                                        menuController.AnchorItem?.ItemDefinition?.Id.ToString("B"));
                                    if (!menuController.IsVisible)
                                        element.SetAttribute("IsVisible", false.ToString());

                                    //if (!(definition.CommandDefinition is CommandMenuControllerDefinition controllerDefinition))
                                    //    return;
                                    //foreach (var item in controllerDefinition.Model.Items)
                                    //{
                                    //    var innerItemElement = document.CreateElement("ItemDefinition", string.Empty,
                                    //        new KeyValuePair<string, string>("Command",
                                    //            item.Key.Id.ToString("B")));
                                    //    element.AppendChild(innerItemElement);
                                    //}
                                });
                            break;
                        case ComboBoxDataSource comboItemDefinition:
                            itemElement = CreateElement(document, "ComboBoxDefinition", comboItemDefinition,
                                element =>
                                {
                                    element.SetAttribute("Command",
                                        comboItemDefinition.ItemDefinition.Id.ToString("B"));
                                    element.SetAttribute("Flags",
                                        ((int)comboItemDefinition.Flags.AllFlags).ToString());
                                    element.SetAttribute("IsEditable",
                                        comboItemDefinition.IsEditable.ToString());
                                    element.SetAttribute("DropDownWidth", comboItemDefinition.DropDownWidth.ToString(InvariantCulture));
                                    if (!comboItemDefinition.IsVisible)
                                        element.SetAttribute("IsVisible", false.ToString());
                                });
                            break;
                        case SplitButtonDataSource splitItemDefinition:
                            itemElement = CreateElement(document,
                                "SplitButtonDefinition", splitItemDefinition,
                                element =>
                                {
                                    element.SetAttribute("Command",
                                        splitItemDefinition.ItemDefinition.Id.ToString("B"));
                                    if (!splitItemDefinition.IsVisible)
                                        element.SetAttribute("IsVisible", false.ToString());
                                });
                            break;
                        case CommandBarItemDataSource commandItem:
                            itemElement = CreateElement(document, "ItemDefinition", commandItem,
                                element =>
                            {
                                element.SetAttribute("Command",
                                    commandItem.ItemDefinition.Id.ToString("B"));
                                if (!commandItem.IsVisible)
                                    element.SetAttribute("IsVisible", false.ToString());
                            });
                            break;
                    }
                    ExplodeGroups(groupItem, itemElement, document);
                    groupElement.AppendChild(itemElement);
                }


                parentXmlElement.AppendChild(groupElement);
            }
        }

        private static XmlElement CreateElement(XmlDocument document, string name, CommandBarDataSource commandBarDefinition,
            Action<XmlElement> fillElementFunc = null)
        {
            var element = document.CreateElement(name);

            element.SetAttribute("Id", commandBarDefinition.Id.ToString("B")); 
            element.SetAttribute("Flags", ((int)commandBarDefinition.Flags.AllFlags).ToString());

            if (commandBarDefinition is ISortable sortable)
                element.SetAttribute("SortOrder", sortable.SortOrder.ToString());

            if (commandBarDefinition.IsTextModified || commandBarDefinition.IsCustom)
                element.SetAttribute("Text", commandBarDefinition.Text);

            fillElementFunc?.Invoke(element);

            return element;
        }

        #endregion

        private T FindCommandBarDefinitionById<T>(Guid guid) where T : CommandBarDataSource
        {
            var definition = _allDefinitions.FirstOrDefault(x => x.Id.Equals(guid));
            return (T)definition;
        }

        private static CommandBarGroup CreateGroup(CommandBarDataSource parent, uint sortOrder)
        {
            return new CommandBarGroup(parent, sortOrder);
        }

        private static void AssignGroup(CommandBarItemDataSource item, CommandBarDataSource parentDefinition)
        {
            item.ContainedGroups.Clear();
            if (parentDefinition is CommandBarGroup parentGroup)
                item.Group = parentGroup;
        }

        private static void SetFlags(CommandBarDataSource item, XmlNode node)
        {
            var flags = node.GetAttributeValue<int>("Flags");
            item.Flags.EnableStyleFlags((CommandBarFlags)flags);
        }
    }
}