using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;
using ModernApplicationFramework.Extended.Interfaces;

namespace ModernApplicationFramework.Modules.Toolbox.Interfaces
{
    public interface IToolboxItem : IToolboxNode
    {
        IToolboxCategory Parent { get; set; }

        IToolboxCategory OriginalParent { get; }

        IDataObject Data { get; }

        BitmapSource IconSource { get; set; }

        TypeArray<ILayoutItem> CompatibleTypes { get; }

        bool Serializable { get; set; }

        bool IsVisible { get; }

        void EvaluateVisibility(Type targetType);
    }

    public class TypeArray<T>
    {
        public IList<Type> Memebers { get; }

        public TypeArray(IEnumerable<Type> input)
        {
            Memebers = new List<Type>();
            foreach (var type in input)
            {
                if (IsSubclassOfRawGeneric(typeof(T), type))
                    Memebers.Add(type);
            }
        }

        static bool IsSubclassOfRawGeneric(Type generic, Type toCheck)
        {
            if (toCheck == generic)
                return true;
            if (generic.IsAssignableFrom(toCheck))
                return true;
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                {
                    return true;
                }
                toCheck = toCheck.BaseType;
            }
            return false;
        }
    }
}