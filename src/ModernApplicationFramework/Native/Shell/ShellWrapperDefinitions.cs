using System.Runtime.InteropServices;

namespace ModernApplicationFramework.Native.Shell
{
    [ComImport, Guid(IidGuid.FileOpenDialog), CoClass(typeof(FileOpenDialogRCW))]
    internal interface NativeFileOpenDialog : IFileOpenDialog {}


    [ComImport, ClassInterface(ClassInterfaceType.None), TypeLibType(TypeLibTypeFlags.FCanCreate), Guid(ClsidGuid.FileOpenDialog)]
    internal class FileOpenDialogRCW {}

    [ComImport,
     ClassInterface(ClassInterfaceType.None),
     TypeLibType(TypeLibTypeFlags.FCanCreate),
     Guid(ClsidGuid.FileSaveDialog)]
    internal class FileSaveDialogRCW
    {
    }

    [ComImport,
     Guid(IidGuid.FileSaveDialog),
     CoClass(typeof(FileSaveDialogRCW))]
    internal interface NativeFileSaveDialog : IFileSaveDialog
    {
    }

}