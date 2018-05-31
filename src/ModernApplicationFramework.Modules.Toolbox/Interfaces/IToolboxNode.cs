using System;
using System.ComponentModel;

namespace ModernApplicationFramework.Modules.Toolbox.Interfaces
{
    public interface IToolboxNode : INotifyPropertyChanged
    {
        string Name { get; }

        Guid Id { get; }

        bool IsSelected { get; set; }

        bool IsExpanded { get; set; }
    }
}