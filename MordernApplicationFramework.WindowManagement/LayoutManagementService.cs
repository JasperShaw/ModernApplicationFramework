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
    public class LayoutManagementService : DisposableObject
    {
        private readonly WindowProfileManager _profileManager;
        private ILayoutManager _layoutManager;
        private WindowProfile.WindowProfile _activeProfile;

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
                if (_activeProfile != null && _activeProfile.Equals(value))
                    return;
                _profileManager.AddProfile(value);
                _activeProfile = value;
                using (var stream = LayoutManagementUtilities.ConvertLayoutPayloadToStream(value.DecompressedPayload))
                    LayoutItemStatePersister.Instance.LoadFromStream(stream, ProcessStateOption.ToolsOnly);
            }
        }

        public IReadOnlyCollection<WindowProfile.WindowProfile> LoadedProfiles => _profileManager.Profiles.ToList();

        public LayoutManagementService()
        {
            Instance = this;
            _profileManager = new WindowProfileManager(ProfileRootDirectory);
        }

        internal WindowProfile.WindowProfile CreateProfileFromDefaultLayoutOrRegistry(string profileName)
        {
            var state = LayoutManagement.LayoutManager.GetCurrentLayoutData();
            return new WindowProfile.WindowProfile(profileName, state);
        }


        public void SaveActiveFrameLayoutEx(uint grfOptions)
        {
            ThrowIfDisposed();
            if (ActiveProfile == null)
                return;
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
                    //_profileManager.Backup(profile.Name);
                }
                if (activeProfile != null)
                {
                    var currentPayload = LayoutManagement.LayoutManager.GetCurrentLayoutData();
                    activeProfile.DecompressedPayload = currentPayload;
                    _profileManager.Save(activeProfile);
                }
                   
                ActiveProfile = profile;
            }
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
            var layoutStore = new WindowLayoutStore(settingsManager);
            var layoutSettings = new WindowLayoutSettings(settingsManager);
            var statusBar = IoC.Get<IStatusBarDataModelService>();
            var statePersiter = IoC.Get<ILayoutItemStatePersister>();
            _layoutManager = new LayoutManager(statusBar, layoutSettings, layoutStore, statePersiter);
        }
    }
}
