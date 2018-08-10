using System.Runtime.InteropServices;

namespace ModernApplicationFramework.TextEditor
{
    internal interface IGetManagedObject
    {
        GCHandle GetManagedObject();
    }
}