using System.Runtime.InteropServices;

namespace ModernApplicationFramework.Core.Shell
{
    [ComImport, Guid(IidGuid.FileOpenDialog), CoClass(typeof(FileOpenDialogRCW))]
    internal interface NativeFileOpenDialog : IFileOpenDialog {}


    [ComImport, ClassInterface(ClassInterfaceType.None), TypeLibType(TypeLibTypeFlags.FCanCreate), Guid(ClsidGuid.FileOpenDialog)]
    internal class FileOpenDialogRCW {}

}