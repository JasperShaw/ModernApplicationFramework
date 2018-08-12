using System;
using System.Collections.Generic;
using System.Linq;
using ModernApplicationFramework.Utilities.Interfaces;

namespace ModernApplicationFramework.Modules.Editor.Utilities
{
    internal static class StableOrderer
    {
        private static bool OrderDependencyFunction<TValue, TMetadata>(Lazy<TValue, TMetadata> x, Lazy<TValue, TMetadata> y) where TValue : class where TMetadata : IOrderable
        {
            var metadata = y.Metadata;
            var before = metadata.Before;
            int num1;
            if (before == null)
            {
                num1 = 0;
            }
            else
            {
                metadata = x.Metadata;
                num1 = before.Contains(metadata.Name) ? 1 : 0;
            }
            if (num1 != 0)
                return true;
            metadata = x.Metadata;
            var after = metadata.After;
            int num2;
            if (after == null)
            {
                num2 = 0;
            }
            else
            {
                metadata = y.Metadata;
                num2 = after.Contains(metadata.Name) ? 1 : 0;
            }
            return num2 != 0;
        }

        public static IEnumerable<Lazy<TValue, TMetadata>> Order<TValue, TMetadata>(IEnumerable<Lazy<TValue, TMetadata>> itemsToOrder) where TValue : class where TMetadata : IOrderable
        {
            return StableTopologicalSort.Order(itemsToOrder, OrderDependencyFunction);
        }
    }
}