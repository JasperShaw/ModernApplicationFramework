using System.Runtime.InteropServices;
using ModernApplicationFramework.Core.Interop;

namespace ModernApplicationFramework.Core.Shell
{
    internal interface INativeCommonFileDialog {}

    // ---------------------------------------------------------
    // Coclass interfaces - designed to "look like" the object 
    // in the API, so that the 'new' operator can be used in a 
    // straightforward way. Behind the scenes, the C# compiler
    // morphs all 'new CoClass()' calls to 'new CoClassWrapper()'
    [ComImport,
     Guid(IidGuid.FileOpenDialog),
     CoClass(typeof(FileOpenDialogRCW))]
    internal interface NativeFileOpenDialog : IFileOpenDialog {}

    [ComImport,
     Guid(IidGuid.FileSaveDialog),
     CoClass(typeof(FileSaveDialogRCW))]
    internal interface NativeFileSaveDialog : IFileSaveDialog {}

    [ComImport,
     Guid(IidGuid.KnownFolderManager),
     CoClass(typeof(KnownFolderManagerRCW))]
    internal interface KnownFolderManager : IKnownFolderManager {}

    [ComImport,
     ClassInterface(ClassInterfaceType.None),
     TypeLibType(TypeLibTypeFlags.FCanCreate),
     Guid(ClsidGuid.FileOpenDialog)]
    internal class FileOpenDialogRCW {}

    [ComImport,
     ClassInterface(ClassInterfaceType.None),
     TypeLibType(TypeLibTypeFlags.FCanCreate),
     Guid(ClsidGuid.FileSaveDialog)]
    internal class FileSaveDialogRCW {}

    [ComImport,
     ClassInterface(ClassInterfaceType.None),
     TypeLibType(TypeLibTypeFlags.FCanCreate),
     Guid(ClsidGuid.KnownFolderManager)]
    internal class KnownFolderManagerRCW {}
}