using System.Runtime.InteropServices;

namespace ModernApplicationFramework.Editor.Implementation
{
    internal interface IGetManagedObject
    {
        GCHandle GetManagedObject();
    }
}