using System;
using Caliburn.Micro;

namespace ModernApplicationFramework.Modules.Toolbox.Interfaces
{
    public interface IToolboxCategory : IToolboxNode
    {
        Type TargetType { get; }

        IObservableCollection<IToolboxItem> Items { get; set; }

        bool HasItems { get; set; }

        void EvaluateItems(Type targetType);
    }
}