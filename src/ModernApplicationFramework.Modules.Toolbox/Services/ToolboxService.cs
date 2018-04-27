using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;

namespace ModernApplicationFramework.Modules.Toolbox.Services
{
    //[Export(typeof(IToolboxService))]
    //public class ToolboxService : IToolboxService
    //{
    //    private readonly Dictionary<Type, IEnumerable<ToolboxItem>> _toolboxItems;

    //    public ToolboxService()
    //    {
    //        _toolboxItems = AssemblySource.Instance
    //            .SelectMany(x => x.GetTypes().Where(y => y.GetAttributes<ToolboxItemAttribute>(false).Any()))
    //            .Select(x =>
    //            {
    //                var attribute = x.GetAttributes<ToolboxItemAttribute>(false).First();
    //                return new ToolboxItem
    //                {
    //                    LayoutItemType = attribute.LayoutItemType,
    //                    Name = attribute.Name,
    //                    Category = attribute.Category,
    //                    IconSource = attribute.IconSource != null ? new Uri(attribute.IconSource) : null,
    //                    ItemType = x,
    //                };
    //            })
    //            .GroupBy(x => x.LayoutItemType)
    //            .ToDictionary(x => x.Key, x => x.AsEnumerable());
    //    }

    //    public IEnumerable<ToolboxItem> GetToolboxItems(Type layoutItemType)
    //    {
    //        if (!_toolboxItems.TryGetValue(layoutItemType, out var result))
    //            return new List<ToolboxItem>();
    //        return result;
    //    }
    //}
}
