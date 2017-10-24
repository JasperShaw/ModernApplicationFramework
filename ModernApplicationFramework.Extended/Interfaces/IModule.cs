using System;
using System.Collections.Generic;
using System.Windows;

namespace ModernApplicationFramework.Extended.Interfaces
{
    public interface IModule : IDisposable
    {
        IEnumerable<ILayoutItem> DefaultDocuments { get; }

        IEnumerable<Type> DefaultTools { get; }

        IEnumerable<ResourceDictionary> GlobalResourceDictionaries { get; }

        void Initialize();

        void PostInitialize();

        void PreInitialize();
    }
}