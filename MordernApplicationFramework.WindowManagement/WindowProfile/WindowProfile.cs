using System;
using System.Text;
using ModernApplicationFramework.Utilities;

namespace MordernApplicationFramework.WindowManagement.WindowProfile
{
    public class WindowProfile
    {
        public string Name { get; private set; }

        public string StatePlayload { get; private set; }

        internal string DecompressedPayload
        {
            get
            {
                try
                {
                    return Encoding.UTF8.GetString(GZip.Decompress(Convert.FromBase64String(StatePlayload)));
                }
                catch
                {
                    //Ignored
                }
                return string.Empty;
            }
            set => StatePlayload = Convert.ToBase64String(GZip.Compress(Encoding.UTF8.GetBytes(value)));
        }

        private WindowProfile()
        {
            
        }

        public WindowProfile(string name, string payload, bool compress = true)
        {
            Name = name;
            StatePlayload = compress ? Convert.ToBase64String(GZip.Compress(Encoding.UTF8.GetBytes(payload))) : payload;
        }


        public static WindowProfile Create(string profileName)
        {
            var profile = new WindowProfile();
            profile.Name = profileName;
            return profile;
        }


        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj is WindowProfile profile)
                return Equals(profile);
            return false;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public bool Equals(WindowProfile profile)
        {
            return Name.Equals(profile.Name, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}