using System.Collections.Generic;
using System.IO;
using System.Linq;
using Caliburn.Micro;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Utilities;
using ModernApplicationFramework.Utilities.Interfaces.Settings;
using MordernApplicationFramework.WindowManagement.LayoutManagement;
using MordernApplicationFramework.WindowManagement.LayoutState;
using MordernApplicationFramework.WindowManagement.WindowProfile;

namespace MordernApplicationFramework.WindowManagement
{
    internal class LayoutManagementService : DisposableObject, ILayoutManagementService
    {
        private readonly WindowProfileManager _profileManager;
        private ILayoutManager _layoutManager;
        private WindowProfile.WindowProfile _activeProfile;
        private readonly LayoutItemStatePersister _statePersiter;

        public static LayoutManagementService Instance { get; private set; }

        private static string ProfileRootDirectory
        {
            get
            {
                var pvar = IoC.Get<IApplicationEnvironment>().AppDataPath;
                return Path.Combine(pvar, "WindowLayouts");
            }
        }

        internal ILayoutManager LayoutManager
        {
            get
            {
                if (_layoutManager == null)
                    InitializeLayoutManagement();
                return _layoutManager;
            }
        }

        internal WindowProfile.WindowProfile ActiveProfile
        {
            get => _activeProfile;
            set
            {
                _activeProfile = value;
                using (var stream = LayoutManagementUtilities.ConvertLayoutPayloadToStream(value.DecompressedPayload))
                    _statePersiter.LoadFromStream(stream, ProcessStateOption.ToolsOnly);
            }
        }

        internal IReadOnlyCollection<WindowProfile.WindowProfile> LoadedProfiles => _profileManager.Profiles.ToList();

        public LayoutManagementService()
        {
            Instance = this;
            _profileManager = new WindowProfileManager(ProfileRootDirectory);

            _statePersiter = new LayoutItemStatePersister();
            _statePersiter.Initialize();
        }

        internal WindowProfile.WindowProfile CreateProfileFromDefaultLayoutOrRegistry(string profileName)
        {
            var defaultProfileProvider = IoC.Get<IDefaultWindowProfileProvider>();
            var profile = defaultProfileProvider?.GetLayout(profileName);
            if (profile != null)
                return profile;
            var state = SaveFrameLayoutCollection(ProcessStateOption.ToolsOnly); 
            return new WindowProfile.WindowProfile(profileName, state);
        }

        internal string SaveFrameLayoutCollection(ProcessStateOption toolsOnly)
        {
            using (var memoryStream = new MemoryStream())
            {
                LayoutItemStatePersister.Instance.SaveToStream(memoryStream, ProcessStateOption.ToolsOnly);
                memoryStream.Seek(0L, SeekOrigin.Begin);
                var byteArray = memoryStream.ToArray();
                return LayoutManagementUtilities.ConvertLayoutStreamToString(byteArray);
            }
        }


        public void SaveActiveFrameLayout()
        {
            ThrowIfDisposed();
            if (ActiveProfile == null)
                return;
            var currentPayload = SaveFrameLayoutCollection(ProcessStateOption.ToolsOnly);
            ActiveProfile.DecompressedPayload = currentPayload;
            _profileManager.Save(ActiveProfile);
        }


        public void LoadLayout(string profileName)
        {
            ThrowIfDisposed();
            var activeProfile = ActiveProfile;
            if (activeProfile == null || activeProfile.Name != profileName)
            {
                var profile = _profileManager.GetProfile(profileName);
                if (profile == null)
                {
                    profile = _profileManager.CreateProfile(profileName, CreateProfileFromDefaultLayoutOrRegistry);
                    _profileManager.Save(profile);
                    _profileManager.Backup(profile.Name);
                    _profileManager. AddProfile(profile);
                }
                if (activeProfile != null)
                {
                    var currentPayload = SaveFrameLayoutCollection(ProcessStateOption.ToolsOnly);
                    activeProfile.DecompressedPayload = currentPayload;
                    _profileManager.Save(activeProfile);          
                }           
                ActiveProfile = profile;
            }
        }

        public void RestoreFrameLayoutCollection(Stream stream)
        {
            LayoutItemStatePersister.Instance.LoadFromStream(stream, ProcessStateOption.ToolsOnly);
            Reload(false);
        }

        public void Reload()
        {
            Reload(true);
        }

        private void Reload(bool createBackup)
        {
            ThrowIfDisposed();
            _profileManager.Reload(ActiveProfile.Name);
            if (!createBackup)
                return;
            _profileManager.Backup(ActiveProfile.Name);
        }

        internal void Initialize()
        {
            ConnectEvent();
        }

        protected override void DisposeManagedResources()
        {
            DisconnectEvents();
            //FocusTracker.Instance.Dispose();
            base.DisposeManagedResources();
        }

        private void ConnectEvent()
        {
            _profileManager.ProfileSet += ProfileManager_ProfileSet;
        }

        private void DisconnectEvents()
        {
            _profileManager.ProfileSet -= ProfileManager_ProfileSet;
        }

        private void ProfileManager_ProfileSet(object sender, WindowProfileEventArgs args)
        {
            if (ActiveProfile == null || !Equals(ActiveProfile, args.WindowProfile))
                return;
            ActiveProfile = args.WindowProfile;
        }

        private void InitializeLayoutManagement()
        {
            var settingsManager = IoC.Get<ISettingsManager>();
            var layoutStore = IoC.Get<IWindowLayoutStore>();
            var layoutSettings = new WindowLayoutSettings(settingsManager);
            var statusBar = IoC.Get<IStatusBarDataModelService>();
            _layoutManager = new LayoutManager(this, statusBar, layoutSettings, layoutStore);
        }

        internal void RestoreProfiles()
        {
            _profileManager.RestoreProfilesFromBackup();
        }
    }
}
