using System.ComponentModel.Composition;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;
using ModernApplicationFramework.EditorBase.Interfaces.Services;

namespace ModernApplicationFramework.EditorBase.Services
{
    [Export(typeof(IUniqueNameCreator<ISupportedFileDefinition>))]
    internal sealed class UniqueFileNameProvider : UniqueNameCreator<ISupportedFileDefinition>
    {
        public override string GetUniqueName(ISupportedFileDefinition fileDefinition)
        {
            return $"{base.GetUniqueName(fileDefinition)}{fileDefinition.FileExtension}";
        }
    }
}