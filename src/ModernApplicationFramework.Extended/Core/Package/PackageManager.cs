using System;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;

namespace ModernApplicationFramework.Extended.Core.Package
{
    public class PackageManager : IPackageManager
    {
        private bool _initialized;

        private IDictionary<string, IMafPackage> Packages { get; }

        public static IPackageManager Instance { get; private set; }

        public PackageManager()
        {
            Packages = new Dictionary<string, IMafPackage>();
            Instance = this;
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
            }
            _initialized = true;
        }

        public void ShutDown()
        {
            EnsureInitialized();
            foreach (var mafPackage in Packages.Values)
                mafPackage.Close();
        }

        public void LoadPackages(PackageLoadOption loadOption)
        {
            foreach (var mafPackage in Packages.Where(x => x.Value.LoadOption == loadOption))
                if (!mafPackage.Value.Initialized)
                    mafPackage.Value.Initialize();
        }

        public void LoadPackage(Guid id)
        {
            if (!Packages.TryGetValue(id.ToString("N"), out var package))
                throw new ArgumentException("Package was not found");
            package.Initialize();
        }

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

        public bool IsPackageLoaded(Guid id)
        {
            if (!Packages.TryGetValue(id.ToString("N"), out var package))
                throw new ArgumentException("Package was not found");
            return package.Initialized;
        }

        public void ClosePackages(PackageCloseOption option)
        {
            foreach (var mafPackage in Packages.Values.Where(x => x.CloseOption == option))
                if (mafPackage.Initialized)
                    mafPackage.Close();
        }

        public void ClosePackage(Guid id)
        {
            if (!Packages.TryGetValue(id.ToString("N"), out var package))
                throw new ArgumentException("Package was not found");
            if (package.Initialized)
                package.Close();
        }

        private void EnsureInitialized()
        {
            if (_initialized)
                return;
            Initialize();
        }
    }

    public interface IPackageManager
    {
        void Initialize();

        void ShutDown();

        void LoadPackages(PackageLoadOption option);

        void LoadPackage(Guid id);

        void AddSinglePackage(IMafPackage package, bool initialize);

        bool IsPackageLoaded(Guid id);

        void ClosePackages(PackageCloseOption option);

        void ClosePackage(Guid id);
    }

    public enum PackageLoadOption
    {
        Custom,
        OnApplicationStart,
        PreviewWindowLoaded,
        OnMainWindowLoaded,
    }

    public enum PackageCloseOption
    {
        Custom,
        OnMainWindowClosed,
        PreviewApplicationClosed
    }
}
