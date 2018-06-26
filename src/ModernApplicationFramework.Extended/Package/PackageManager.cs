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

        public void AddSinglePackage(IMafPackage package, bool initialize)
        {
            if (package.LoadOption != PackageLoadOption.Custom)
                throw new InvalidOperationException("Only can load packages that have custom load option set");
            var id = package.Id.ToString("N");
            if (Packages.ContainsKey(id))
                throw new InvalidOperationException("Can not add duplicate Package");
            Packages.Add(id, package);
            if (!initialize)
                return;
            LoadPackage(package.Id);
        }

        public void ClosePackage(Guid id)
        {
            if (!Packages.TryGetValue(id.ToString("N"), out var package))
                throw new ArgumentException("Package was not found");
            if (package.Initialized)
                package.Close();
        }

        public void ClosePackages(PackageCloseOption option)
        {
            foreach (var mafPackage in Packages.Values.Where(x => x.CloseOption == option))
                if (mafPackage.Initialized)
                    mafPackage.Close();
        }

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
                this.RegisterAutoLoadPackages(mafPackage, () => LoadPackage(mafPackage.Id));
            }
            _initialized = true;
        }

        public bool IsPackageLoaded(Guid id)
        {
            if (!Packages.TryGetValue(id.ToString("N"), out var package))
                throw new ArgumentException("Package was not found");
            return package.Initialized;
        }

        public void LoadApplicationStartPackages()
        {
            foreach (var mafPackage in Packages.Where(x => x.Value.LoadOption == PackageLoadOption.OnApplicationStart))
                if (!mafPackage.Value.Initialized)
                    ((Package) mafPackage.Value).InitializeInternal();
        }

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

        private void LoadPackage(Guid id)
        {
            if (!Packages.TryGetValue(id.ToString("N"), out var package))
                throw new ArgumentException("Package was not found");

            LoadPackage(package);
        }


        private void LoadPackage(IMafPackage package)
        {
            ((Package)package).InitializeInternal();
        }
    }
}