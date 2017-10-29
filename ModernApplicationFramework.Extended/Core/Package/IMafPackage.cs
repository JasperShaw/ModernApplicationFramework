using System;
using System.Collections.Generic;
using System.Windows;

namespace ModernApplicationFramework.Extended.Core.Package
{
    public interface IMafPackage
    {
        PackageLoadOption LoadOption { get; }

        PackageCloseOption CloseOption { get; }

        bool Initialized { get; }

        Guid Id { get; }

        IEnumerable<Type> DefaultTools { get; }

        IEnumerable<ResourceDictionary> GlobalResourceDictionaries { get; }

        void Close();

        void Initialize();
    }
}