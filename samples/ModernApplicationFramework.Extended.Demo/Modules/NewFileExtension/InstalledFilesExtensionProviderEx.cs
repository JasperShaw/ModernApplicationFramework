using System.ComponentModel.Composition;
using ModernApplicationFramework.EditorBase.Commands;

namespace ModernApplicationFramework.Extended.Demo.Modules.NewFileExtension
{
    //[Export(typeof(InstalledFilesExtensionProvider))]
    public class InstalledFilesExtensionProviderEx : InstalledFilesExtensionProvider
    {
        public override string Text => "Installed Ex";
    }
}
