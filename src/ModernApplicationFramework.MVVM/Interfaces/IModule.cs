using System;
using System.Collections.Generic;
using System.Windows;

namespace ModernApplicationFramework.MVVM.Interfaces
{
    public interface IModule
    {
        IEnumerable<IDocument> DefaultDocuments { get; }
        IEnumerable<Type> DefaultTools { get; }
        IEnumerable<ResourceDictionary> GlobalResourceDictionaries { get; }
        void Initialize();
        void PostInitialize();

        void PreInitialize();
    }
}