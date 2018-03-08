using System;
using System.Threading.Tasks;
using ModernApplicationFramework.EditorBase.FileSupport;

namespace ModernApplicationFramework.EditorBase.Interfaces.FileSupport
{
    public interface IStorableFile : IFile
    {
        event EventHandler DirtyChanged; 

        bool IsDirty { get; set; }

        bool IsNew { get; }

        Task Save(SaveFileArguments arguments, Action saveAction);
    }
}