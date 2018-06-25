using System;
using System.Collections.Generic;
using System.Windows;

namespace ModernApplicationFramework.Extended.Package
{
    /// <summary>
    /// Interface for application modules
    /// </summary>
    public interface IMafPackage
    {
        /// <summary>
        /// The load option.
        /// </summary>
        PackageLoadOption LoadOption { get; }

        /// <summary>
        /// The close option.
        /// </summary>
        PackageCloseOption CloseOption { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="IMafPackage"/> is initialized.
        /// </summary>
        bool Initialized { get; }

        /// <summary>
        /// The identifier.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// The default tools of this <see cref="IMafPackage"/>.
        /// </summary>
        IEnumerable<Type> DefaultTools { get; }

        /// <summary>
        /// Global resource dictionaries of the <see cref="IMafPackage"/>.
        /// </summary>
        IEnumerable<ResourceDictionary> GlobalResourceDictionaries { get; }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        void Close();

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        //void Initialize();
    }
}