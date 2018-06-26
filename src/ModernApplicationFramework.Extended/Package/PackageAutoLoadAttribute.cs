using System;

namespace ModernApplicationFramework.Extended.Package
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class PackageAutoLoadAttribute : Attribute
    {
        public Guid LoadGuid { get; }

        public PackageAutoLoadAttribute(string uiContextGuid)
        {
            LoadGuid = new Guid(uiContextGuid);
        }
    }
}
