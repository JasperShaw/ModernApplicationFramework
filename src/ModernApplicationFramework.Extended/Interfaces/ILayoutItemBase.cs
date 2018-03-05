using System;
using System.IO;
using Caliburn.Micro;

namespace ModernApplicationFramework.Extended.Interfaces
{
    public interface ILayoutItemBase : IScreen, IViewAware
    {
        string ContentId { get; }

        Uri IconSource { get; }

        Guid Id { get; }

        bool ShouldReopenOnStart { get; }

        bool IsSelected { get; set; }

        void LoadState(BinaryReader reader);

        void SaveState(BinaryWriter writer);
    }
}