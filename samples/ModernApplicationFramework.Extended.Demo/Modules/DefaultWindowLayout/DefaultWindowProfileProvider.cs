using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Reflection;
using ModernApplicationFramework.WindowManagement.LayoutState;
using ModernApplicationFramework.WindowManagement.Utilities;
using ModernApplicationFramework.WindowManagement.WindowProfile;

namespace ModernApplicationFramework.Extended.Demo.Modules.DefaultWindowLayout
{
    [Export(typeof(IDefaultWindowProfileProvider))]
    public class DefaultWindowProfileProvider : IDefaultWindowProfileProvider
    {
        private readonly Dictionary<string, WindowProfile> _profiles;

        public DefaultWindowProfileProvider()
        {
            _profiles = new Dictionary<string, WindowProfile>();

            Setup();
        }

        public WindowProfile GetLayout(string profileName)
        {
            _profiles.TryGetValue(profileName, out var profile);
            return profile;
        }

        private void Setup()
        {
            using (var stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("ModernApplicationFramework.Extended.Demo.Resources.DefaultProfile.winprf"))
            {
                var payload = LayoutPayloadUtilities.StreamToPlayloadData(stream);
                var profile = new WindowProfile("Default", payload);
                _profiles.Add("Default", profile);
            }   
        }
    }
}
