using System;
using System.Threading.Tasks;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.EditorBase.Interfaces.Editor
{
    public interface IEditor : ILayoutItem, ICanHaveInputBindings
    {
        Guid EditorId { get; }

        string LocalizedName { get; }

        string Name { get; }

        IFile Document { get; }

        bool IsReadOnly { get; }

        void LoadFile(IFile document, string name);

        Task Reload();

        void SaveFile(bool saveAs = false);

        bool CanHandleFile(ISupportedFileDefinition fileDefinition);
    }
}