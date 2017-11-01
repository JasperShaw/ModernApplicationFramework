using System;
using System.Text;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.WindowManagement.WindowProfile
{
    /// <summary>
    /// A <see cref="WindowProfile"/> contains information about a window layout configuration
    /// </summary>
    public class WindowProfile
    {
        /// <summary>
        /// The name of the profile. The name is unique in an application
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The compressed payload data that holds the window layout
        /// </summary>
        public string StatePlayload { get; private set; }

        /// <summary>
        /// The decompressed payload data. Settings this property will compress the payload and fills the <see cref="StatePlayload"/> property.
        /// </summary>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowProfile"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="payload">The payload.</param>
        /// <param name="compress">if set to <see langword="true"/> the payload data will be compressed first. Default value is <see langword="true"/>.</param>
        public WindowProfile(string name, string payload, bool compress = true)
        {
            Name = name;
            StatePlayload = compress ? Convert.ToBase64String(GZip.Compress(Encoding.UTF8.GetBytes(payload))) : payload;
        }


        internal static WindowProfile Create(string profileName)
        {
            var profile = new WindowProfile {Name = profileName};
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