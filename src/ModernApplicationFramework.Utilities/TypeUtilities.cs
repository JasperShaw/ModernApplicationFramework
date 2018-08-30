using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ModernApplicationFramework.Utilities
{
    public static class TypeUtilities
    {
        public static IEnumerable CreateDynamicallyTypedList(IEnumerable source)
        {
            var type = GetCommonBaseClass(source);
            var listType = typeof(List<>).MakeGenericType(type);
            var addMethod = listType.GetMethod("Add");
            var list = listType.GetConstructor(Type.EmptyTypes)?.Invoke(null);

            foreach (var o in source)
            {
                if (addMethod != null) addMethod.Invoke(list, new[] {o});
            }

            return (IEnumerable)list;
        }

        public static Type GetCommonBaseClass(IEnumerable e)
        {
            var types = e.Cast<object>().Select(o => o.GetType()).ToArray();
            return GetCommonBaseClass(types);
        }

        public static Type GetCommonBaseClass(Type[] types)
        {
            if (types.Length == 0)
            {
                return typeof(object);
            }

            var ret = types[0];

            for (var i = 1; i < types.Length; ++i)
            {
                if (types[i].IsAssignableFrom(ret))
                {
                    ret = types[i];
                }
                else
                {
                    // This will always terminate when ret == typeof(object)
                    while (!ret.IsAssignableFrom(types[i]))
                    {
                        ret = ret.BaseType;
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// Gets the enumerable as list.
        /// If enumerable is an ICollectionView then it returns the SourceCollection as list.
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        /// <returns>Returns a list.</returns>
        public static IList TryGetList(this IEnumerable enumerable)
        {
            if (enumerable is ICollectionView view)
            {
                return view.SourceCollection as IList;
            }
            var list = enumerable as IList;
            return list ?? enumerable?.OfType<object>().ToList();
        }

        public static bool CrossCheckTypeCompatibility(this Type type, Type other)
        {
            if (type == other)
                return true;
            if (type == null || other == null)
                return false;
            if (other.IsAssignableFrom(type) || type.IsAssignableFrom(other))
                return true;
            if (other.GetInterfaces().Contains(type) || type.GetInterfaces().Contains(other))
                return true;
            return false;
        }

        public static bool ImplementsOrInharits(this Type type, Type other)
        {
            if (type == null || other == null)
                return false;
            if (type == other)
                return true;
            return other.IsAssignableFrom(type) || type.GetInterfaces().Contains(other);
        }


        public static Type GetTypeFromAllLoadedAssemblies(string name)
        {
            return
                AppDomain.CurrentDomain.GetAssemblies()
                    .Where(a => !a.IsDynamic)
                    .SelectMany(a => a.GetTypes())
                    .FirstOrDefault(t => t.FullName != null && t.FullName.Equals(name));
        }
    }
}
