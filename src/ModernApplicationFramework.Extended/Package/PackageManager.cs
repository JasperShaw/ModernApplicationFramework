using System;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;

namespace ModernApplicationFramework.Extended.Package
{
    public sealed class PackageManager : IPackageManager
    {
        private bool _initialized;

        /// <summary>
        /// Gets current instance.
        /// </summary>
        public static IPackageManager Instance { get; private set; }

        private IDictionary<string, IMafPackage> Packages { get; }

        public PackageManager()
        {
            Packages = new Dictionary<string, IMafPackage>();
            Instance = this;
        }

        /// <inheritdoc />
        /// <summary>
        /// Adds a single package to the <see cref="T:ModernApplicationFramework.Extended.Package.IPackageManager" />.
        /// </summary>
        /// <param name="package">The package.</param>
        /// <param name="initialize">if set to <see langword="true" />the package will be initialized</param>
        /// <exception cref="T:System.InvalidOperationException">Can not add duplicate Package</exception>
        public void AddSinglePackage(IMafPackage package, bool initialize)
        {
            var id = package.Id.ToString("N");
            if (Packages.ContainsKey(id))
                throw new InvalidOperationException("Can not add duplicate Package");
            Packages.Add(id, package);
            if (!initialize)
                return;
            LoadPackage(package.Id);
        }

        /// <inheritdoc />
        /// <summary>
        /// Closes a single <see cref="T:ModernApplicationFramework.Extended.Package.IMafPackage" />.
        /// </summary>
        /// <param name="id">The identifier of the <see cref="T:ModernApplicationFramework.Extended.Package.IMafPackage" />.</param>
        /// <exception cref="T:System.ArgumentException">Package was not found</exception>
        public void ClosePackage(Guid id)
        {
            if (!Packages.TryGetValue(id.ToString("N"), out var package))
                throw new ArgumentException("Package was not found");
            if (package.Initialized)
                package.Close();
        }

        /// <inheritdoc />
        /// <summary>
        /// Closes all <see cref="T:ModernApplicationFramework.Extended.Package.IMafPackage" />s matching the specified options.
        /// </summary>
        /// <param name="option">The option.</param>
        public void ClosePackages(PackageCloseOption option)
        {
            foreach (var mafPackage in Packages.Values.Where(x => x.CloseOption == option))
                if (mafPackage.Initialized)
                    mafPackage.Close();
        }

        /// <inheritdoc />
        /// <summary>
        /// Initializes the <see cref="T:ModernApplicationFramework.Extended.Package.IPackageManager" />.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">Can not import Packages twice</exception>
        public void Initialize()
        {
            if (_initialized)
                throw new InvalidOperationException("Can not import Packages twice");
            var packages = IoC.GetAll<IMafPackage>();
            foreach (var mafPackage in packages)
            {
                if (Packages.ContainsKey(mafPackage.Id.ToString("N")))
                    continue;
                Packages.Add(mafPackage.Id.ToString("N"), mafPackage);
            }
            _initialized = true;
        }

        /// <inheritdoc />
        /// <summary>
        /// Determines whether a <see cref="T:ModernApplicationFramework.Extended.Package.IMafPackage" /> is initialized.
        /// </summary>
        /// <param name="id">The identifier of the <see cref="T:ModernApplicationFramework.Extended.Package.IMafPackage" /></param>
        /// <returns>
        ///   <see langword="true" /> if the package is loaded; otherwise, <see langword="false" />.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">Package was not found</exception>
        public bool IsPackageLoaded(Guid id)
        {
            if (!Packages.TryGetValue(id.ToString("N"), out var package))
                throw new ArgumentException("Package was not found");
            return package.Initialized;
        }

        /// <inheritdoc />
        /// <summary>
        /// Loads a single package.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <exception cref="T:System.ArgumentException">Package was not found</exception>
        public void LoadPackage(Guid id)
        {
            if (!Packages.TryGetValue(id.ToString("N"), out var package))
                throw new ArgumentException("Package was not found");
            package.Initialize();
        }

        /// <inheritdoc />
        /// <summary>
        /// Loads the packages.
        /// </summary>
        /// <param name="loadOption">The load option.</param>
        public void LoadPackages(PackageLoadOption loadOption)
        {
            foreach (var mafPackage in Packages.Where(x => x.Value.LoadOption == loadOption))
                if (!mafPackage.Value.Initialized)
                    mafPackage.Value.Initialize();
        }

        /// <inheritdoc />
        /// <summary>
        /// Shuts down the <see cref="T:ModernApplicationFramework.Extended.Package.IPackageManager" /> and closes all <see cref="T:ModernApplicationFramework.Extended.Package.IMafPackage" />s.
        /// </summary>
        public void ShutDown()
        {
            EnsureInitialized();
            foreach (var mafPackage in Packages.Values)
                mafPackage.Close();
        }

        private void EnsureInitialized()
        {
            if (_initialized)
                return;
            Initialize();
        }
    }
}