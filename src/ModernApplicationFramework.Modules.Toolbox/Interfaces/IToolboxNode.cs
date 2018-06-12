using System;
using System.ComponentModel;

namespace ModernApplicationFramework.Modules.Toolbox.Interfaces
{
    public interface IToolboxNode : INotifyPropertyChanged
    {
        event EventHandler CreatedCancelled;

        event EventHandler Created;

        string Name { get; set; }

        string EditingName { get; set; }

        Guid Id { get; }

        bool IsCustom { get; }

        bool IsExpanded { get; set; }

        bool IsInRenameMode { get; set; }

        bool IsNewlyCreated { get; }

        bool IsNameModified { get; }

        void EnterRenameMode();

        void ExitRenameMode();

        void CommitRename();

        bool IsRenameValid(out string errorMessage);

        void Reset();
    }
}