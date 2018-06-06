using System;
using System.ComponentModel;

namespace ModernApplicationFramework.Modules.Toolbox.Interfaces
{
    public interface IToolboxNode : INotifyPropertyChanged
    {
        string Name { get; set; }

        Guid Id { get; }

        bool IsCustom { get; }

        bool IsSelected { get; set; }

        bool IsExpanded { get; set; }

        bool IsInRenameMode { get; set; }

        void EnterRenameMode();

        void ExitRenameMode();

        void CommitRename();

        bool IsRenameValid(out string errorMessage);
    }
}