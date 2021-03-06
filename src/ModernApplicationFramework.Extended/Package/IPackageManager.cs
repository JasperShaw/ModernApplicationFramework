﻿using System;
using ModernApplicationFramework.Extended.Interfaces;

namespace ModernApplicationFramework.Extended.Package
{
    /// <summary>
    /// An <see cref="IPackageManager"/> loads, closes and manages all packages of the application
    /// </summary>
    public interface IPackageManager
    {
        /// <summary>
        /// Initializes the <see cref="IPackageManager"/>.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">Can not import Packages twice</exception>
        void Initialize();

        /// <summary>
        /// Shuts down the <see cref="IPackageManager"/> and closes all <see cref="IMafPackage"/>s.
        /// </summary>
        void ShutDown();

        /// <summary>
        /// Loads all the packages having the load option <see cref="PackageLoadOption.OnApplicationStart"/>.
        /// </summary>
        void LoadApplicationStartPackages();

        /// <summary>
        /// Adds a single package to the <see cref="IPackageManager"/>.
        /// </summary>
        /// <param name="package">The package.</param>
        /// <param name="initialize">if set to <see langword="true"/>the package will be initialized</param>
        /// <exception cref="T:System.InvalidOperationException">Can not add duplicate Package</exception>
        /// <exception cref="T:System.InvalidOperationException">Only can load packages that have custom load option set</exception>
        void AddSinglePackage(IMafPackage package, bool initialize);

        /// <summary>
        /// Determines whether a <see cref="IMafPackage"/> is initialized.
        /// </summary>
        /// <param name="id">The identifier of the <see cref="IMafPackage"/></param>
        /// <returns>
        ///   <see langword="true"/> if the package is loaded; otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentException">Throws when package was not found</exception>
        bool IsPackageLoaded(Guid id);

        /// <summary>
        /// Closes all <see cref="IMafPackage"/>s matching the specified options.
        /// </summary>
        /// <param name="option">The option.</param>
        /// <exception cref="T:System.ArgumentException">Package was not found</exception>
        void ClosePackages(PackageCloseOption option);

        /// <summary>
        /// Closes a single <see cref="IMafPackage"/>.
        /// </summary>
        /// <param name="id">The identifier of the <see cref="IMafPackage"/>.</param>
        /// <exception cref="ArgumentException">Throws when package was not found</exception>
        void ClosePackage(Guid id);
    }
}