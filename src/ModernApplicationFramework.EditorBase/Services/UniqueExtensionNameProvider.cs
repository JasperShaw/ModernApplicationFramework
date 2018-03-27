using System.ComponentModel.Composition;
using ModernApplicationFramework.EditorBase.Interfaces.NewElement;
using ModernApplicationFramework.EditorBase.Interfaces.Services;

namespace ModernApplicationFramework.EditorBase.Services
{
    [Export(typeof(IUniqueNameCreator<IExtensionDefinition>))]
    internal sealed class UniqueExtensionNameProvider : UniqueNameCreator<IExtensionDefinition>
    {

    }
}