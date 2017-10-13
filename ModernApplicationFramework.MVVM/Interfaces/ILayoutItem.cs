using System;
using System.IO;
using System.Windows.Input;
using Caliburn.Micro;

namespace ModernApplicationFramework.MVVM.Interfaces
{
    public interface ILayoutItem : IScreen
    {
        ICommand CloseCommand { get; }

        string ContentId { get; }

        Uri IconSource { get; }

        Guid Id { get; }

        bool ShouldReopenOnStart { get; }

        bool IsSelected { get; set; }

        void LoadState(BinaryReader reader);

        void SaveState(BinaryWriter writer);
    }
}