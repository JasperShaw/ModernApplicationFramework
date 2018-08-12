using System.Runtime.InteropServices;

namespace ModernApplicationFramework.Editor
{
    internal interface IGetManagedObject
    {
        GCHandle GetManagedObject();
    }
}