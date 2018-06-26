using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using ModernApplicationFramework.Extended.UIContext;
using Action = System.Action;

namespace ModernApplicationFramework.Extended.Package
{
    internal static class PackageAutoLoadHelper
    {
        private static readonly Dictionary<Guid, HashSet<Guid>> Mapping = new Dictionary<Guid, HashSet<Guid>>();

        public static void RegisterAutoLoadPackages(this IPackageManager manager, IMafPackage package, Action loadAction)
        {
            if (package.LoadOption != PackageLoadOption.OnContextActivated)
                return;

            var attributes = package.GetType().GetAttributes<PackageAutoLoadAttribute>(true);

            foreach (var attribute in attributes)
            {
                var uiContext = UiContext.FromUiContextGuid(attribute.LoadGuid);
                if (Mapping.ContainsKey(attribute.LoadGuid))
                    Mapping[attribute.LoadGuid].Add(package.Id);
                else
                    Mapping.Add(attribute.LoadGuid, new HashSet<Guid>{package.Id});
                uiContext.WhenActivated(loadAction);
            }
        }
    }
}
