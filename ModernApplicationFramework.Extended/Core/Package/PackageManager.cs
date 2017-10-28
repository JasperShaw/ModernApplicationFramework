using System;
using System.Collections.Generic;
using Caliburn.Micro;

namespace ModernApplicationFramework.Extended.Core.Package
{
    public class PackageManager : IPackageManager
    {
        private bool _initialized;

        private IDictionary<string, IMafPackage> Packages { get; }

        public PackageManager()
        {
            Packages = new Dictionary<string, IMafPackage>();
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
            foreach (var mafPackage in Packages)
                mafPackage.Value.Close();
        }

        public void LoadPackages()
        {
            foreach (var mafPackage in Packages)
                if (!mafPackage.Value.Initialized)
                    mafPackage.Value.Initialize();
        }

        public void AddSinglePackage(IMafPackage package, bool initialize)
        {
            var id = package.Id.ToString("N");
            if (Packages.ContainsKey(id))
                throw new InvalidOperationException("Can not add duplicate Package");
            Packages.Add(id, package);
            if (!initialize)
                return;
            //TODO:
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

        void LoadPackages();

        void AddSinglePackage(IMafPackage package, bool initialize);
    }

    public enum PackageLoadOption
    {
        OnApplicationStart,
        OnMainWindowLoaded,
    }
}
