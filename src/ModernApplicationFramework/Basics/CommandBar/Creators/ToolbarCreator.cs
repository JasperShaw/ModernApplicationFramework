﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.CommandBar.DataSources;
using ModernApplicationFramework.Controls.Buttons;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.Utilities;

namespace ModernApplicationFramework.Basics.CommandBar.Creators
{
    /// <inheritdoc cref="IToolbarCreator" />
    /// <summary>
    /// Implementation of <see cref="IToolbarCreator"/>
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Basics.CommandBar.Creators.CreatorBase" />
    /// <seealso cref="T:ModernApplicationFramework.Interfaces.Utilities.IToolbarCreator" />
    [Export(typeof(IToolbarCreator))]
    public class ToolbarCreator : CreatorBase, IToolbarCreator
    {
        public override void CreateRecursive<T>(ref T itemsControl, CommandBarDataSource itemDefinition)
        {
            
        }

        public override void CreateRecursive<T>(ref T itemsControl, CommandBarDataSource itemDefinition, IReadOnlyList<CommandBarGroup> groups,
            Func<CommandBarGroup,IReadOnlyList<CommandBarItemDataSource>> itemFunc)
        {
            var topItem = GetSingleSubDefinitions(itemDefinition, groups, itemFunc);
            if (!(itemDefinition is ToolBarDataSource))
                return;

            foreach (var item in topItem)
            {
                if (item is MenuDataSource menuDataSource)
                {
                    var list = new ObservableCollection<CommandBarItemDataSource>();
                    Fill(ref list, item, itemFunc);
                    menuDataSource.Items = list;
                    itemsControl.Items.Add(new CommandDefinitionButton(item));
                }
                else
                    itemsControl.Items.Add(new CommandDefinitionButton(item));
            }

        }


        //public override void CreateRecursive<T>(ref T itemsControl, CommandBarDefinitionBase itemDefinition)
        //{
        //    var topItem = GetSingleSubDefinitions(itemDefinition);
        //    if (!(itemDefinition is ToolbarDefinition))
        //        return;
        //    foreach (var item in topItem)
        //    {
        //        if (item is MenuDefinition)
        //        {
        //            var list = new ObservableCollection<CommandBarItemDefinition>();
        //            Fill(ref list, item);
        //            item.Items = list;
        //            itemsControl.Items.Add(new CommandDefinitionButton(item));
        //        }
        //        else
        //            itemsControl.Items.Add(new CommandDefinitionButton(item));
        //    }
        //}

        //public void CreateToolbarDefinition<T>(ref T itemsControl, ToolbarDefinition toolbarDefinition,
        //    Func<IReadOnlyCollection<CommandBarGroupDefinition>> groupAction) where T : ItemsControl
        //{

        //    var groups = groupAction();

        //    var topItem = GetSingleSubDefinitions(toolbarDefinition, groups.ToList(), group => groups.SelectMany(x => x.Items).ToList());

        //    foreach (var item in topItem)
        //    {
        //        if (item is MenuDefinition)
        //        {
        //            var list = new ObservableCollection<CommandBarItemDefinition>();
        //            Fill(ref list, item, );
        //            item.Items = list;
        //            itemsControl.Items.Add(new CommandDefinitionButton(item));
        //        }
        //        else
        //            itemsControl.Items.Add(new CommandDefinitionButton(item));
        //    }
        //}

        private void Fill(ref ObservableCollection<CommandBarItemDataSource> list, CommandBarDataSource item,
            Func<CommandBarGroup, IReadOnlyList<CommandBarItemDataSource>> itemFunc)
        {
            var host = IoC.Get<ICommandBarDefinitionHost>();
            var group = host.ItemGroupDefinitions.Where(x => x.Parent == item)
                .Where(x => !host.ExcludedItemDefinitions.Contains(x))
                .Where(x => x.Items.Any(y => y.IsVisible))
                .OrderBy(x => x.SortOrder)
                .ToList();


            var defs = GetSingleSubDefinitions(item, group, itemFunc);
            foreach (var def in defs)
            {
                list.Add(def);
                if (def is MenuDataSource menuDataSource)
                {
                    var newList = new ObservableCollection<CommandBarItemDataSource>();
                    Fill(ref newList, def, itemFunc);
                    menuDataSource.Items = newList;
                }
            }
        }
    }
}