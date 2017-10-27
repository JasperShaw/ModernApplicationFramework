using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Utilities;
using MordernApplicationFramework.WindowManagement.LayoutState;

namespace MordernApplicationFramework.WindowManagement.WindowProfile
{
    internal class WindowProfileManager
    {
        private string _fileNameIndexFullPath;

        private readonly Dictionary<string, string> _fileNameIndex = new Dictionary<string, string>();


        private bool _isFileNameIndexLoaded;


        public event EventHandler<WindowProfileEventArgs> ProfileAdded;

        public event EventHandler<WindowProfileEventArgs> ProfileSet;

        public WindowProfile ActiveProfile { get; private set; }


        public readonly ICollection<WindowProfile> Profiles = new List<WindowProfile>();

        public void AddProfile(WindowProfile profile)
        {
            if (Profiles.Contains(profile))
            {
                OnProfileSet(profile);
            }
            else
            {
                Profiles.Add(profile);
                GetOrCreateProfileFileName(profile.Name);
                OnProfileAdded(profile);
            }
        }


        private string ProfileRootDirectory { get; }

        private string FileNameIndexFullPath => _fileNameIndexFullPath ?? (_fileNameIndexFullPath = GetFileNameIndexFullPath());


        public WindowProfileManager(string profileRootDirectory)
        {
            Validate.IsNotNull(profileRootDirectory, nameof(profileRootDirectory));
            if (!Path.IsPathRooted(profileRootDirectory))
                throw new ArgumentException("Profile root directory must be a rooted path", nameof(profileRootDirectory));
            ProfileRootDirectory = profileRootDirectory;
        }

        public void Save(WindowProfile profile)
        {
            Validate.IsNotNull(profile, nameof(profile));
            if (!EnsureLocalStorageDirectoryExists())
                return;
            SaveProfileToLocalStorage(profile);
        }



        public WindowProfile GetProfile(string profileName, bool fromBackup = false)
        {
            Validate.IsNotNullAndNotEmpty(profileName, nameof(profileName));
            var profile = default(WindowProfile);
            if ((fromBackup || !TryGetProfile(profileName, out profile)) && IsProfileInIndex(profileName))
                profile = LoadProfileFromLocalStorage(profileName, fromBackup);
            return profile;
        }

        public WindowProfile CreateProfile(string profileName, Func<string, WindowProfile> activator)
        {
            Validate.IsNotNullAndNotEmpty(profileName, nameof(profileName));
            EnsureFileNameIndexLoaded();
            var windowProfile = activator(profileName) ?? WindowProfile.Create(profileName);
            return windowProfile;
        }


        private void SaveProfileToLocalStorage(WindowProfile profile)
        {
            try
            {
                var statePersister = LayoutItemStatePersister.Instance;
                if (statePersister == null)
                    return;
                statePersister.PayloadDataToFile(GetProfileFullPath(profile.Name), profile.StatePlayload);
            }
            catch (IOException ex)
            {
                //this.RaiseExceptionFilter((Exception)ex, "Failed to save window configuration.");
            }
            catch (UnauthorizedAccessException ex)
            {
                //this.RaiseExceptionFilter((Exception)ex, "Failed to save window configuration.");
            }
        }

        public Stream OpenProfileLocalStorage(string profileName, FileAccess fileAccess)
        {
            Validate.IsNotNullAndNotEmpty(profileName, nameof(profileName));
            var mode = fileAccess == FileAccess.Read ? FileMode.Open : FileMode.Create;
            var share = fileAccess == FileAccess.Read ? FileShare.Read : FileShare.None;
            return File.Open(GetProfileFullPath(profileName), mode, fileAccess, share);
        }

        private string GetProfileFullPath(string profileName)
        {
            EnsureFileNameIndexLoaded();
            return GetOrCreateProfileFileName(profileName);
        }

        private WindowProfile LoadProfileFromLocalStorage(string profileName, bool fromBackup)
        {
            try
            {
                var payload = LayoutItemStatePersister.Instance.FileToPayloadData(GetProfileFullPath(profileName));
                return new WindowProfile(profileName, payload);
            }
            catch
            {
                return null;
            }
        }


        protected void OnProfileSet(WindowProfile newProfile)
        {
            ProfileSet?.Invoke(this, new WindowProfileEventArgs(newProfile));
        }

        protected void OnProfileAdded(WindowProfile newProfile)
        {
            ProfileAdded?.Invoke(this, new WindowProfileEventArgs(newProfile));
        }

        private string GetOrCreateProfileFileName(string profileName)
        {
            if (!_fileNameIndex.TryGetValue(profileName, out var profileFileName))
            {
                profileFileName = GenerateProfileFileName(profileName);
                _fileNameIndex[profileName] = profileFileName;
                SaveFileNameIndex();
            }
            return profileFileName;
        }

        private void SaveFileNameIndex()
        {
            if (!EnsureLocalStorageDirectoryExists())
                return;
            SaveFileNameIndexToLocalStorage();
        }

        private bool EnsureLocalStorageDirectoryExists()
        {
            try
            {
                if (!Directory.Exists(ProfileRootDirectory))
                    Directory.CreateDirectory(ProfileRootDirectory);
                return true;
            }
            catch (IOException ex)
            {
                //this.RaiseExceptionFilter((Exception)ex, "Failed to create window configuration storage directory.");
                return false;
            }
            catch (UnauthorizedAccessException ex)
            {
                //this.RaiseExceptionFilter((Exception)ex, "Failed to create window configuration storage directory.");
                return false;
            }
        }

        private string GenerateProfileFileName(string profileName)
        {
            StringBuilder stringBuilder = new StringBuilder(profileName.Length > 50 ? profileName.Substring(0, 50) : profileName);
            foreach (char invalidFileNameChar in Path.GetInvalidFileNameChars())
                stringBuilder.Replace(invalidFileNameChar, '_');
            stringBuilder.Append("_");
            stringBuilder.Append(Path.GetRandomFileName());
            stringBuilder.Append(".winprf");
            return Path.Combine(ProfileRootDirectory, stringBuilder.ToString());
        }

        private void EnsureFileNameIndexLoaded()
        {
            if (_isFileNameIndexLoaded)
                return;
            LoadFileNameIndexFromLocalStorage();
            _isFileNameIndexLoaded = true;
        }

        public bool IsProfileInIndex(string profileName)
        {
            Validate.IsNotNullAndNotEmpty(profileName, nameof(profileName));
            EnsureFileNameIndexLoaded();
            return _fileNameIndex.ContainsKey(profileName);
        }

        private bool TryGetProfile(string profileName, out WindowProfile profile)
        {
            profile = null;
            if (Profiles.All(x => x.Name != profileName))
                return false;
            profile = Profiles.First(x => x.Name == profileName);
            return true;
        }


        private bool LoadFileNameIndexFromLocalStorage()
        {
            try
            {
                if (!File.Exists(FileNameIndexFullPath))
                    return false;
                using (Stream stream = File.Open(FileNameIndexFullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (var streamReader = new StreamReader(stream))
                    {
                        var strArray = streamReader.ReadToEnd().Split(new char[1]);
                        if (strArray.Length % 2 != 0)
                            return false;
                        var index = 0;
                        while (index < strArray.Length)
                        {
                            _fileNameIndex[strArray[index]] = Path.Combine(ProfileRootDirectory, Path.GetFileName(strArray[index + 1]));
                            index += 2;
                        }
                    }
                }
                return true;
            }
            catch (IOException ex)
            {
                return false;
            }
            catch (UnauthorizedAccessException ex)
            {
                return false;
            }
        }


        private void SaveFileNameIndexToLocalStorage()
        {
            try
            {
                using (Stream stream = File.Open(FileNameIndexFullPath, FileMode.Create, FileAccess.Write))
                {
                    using (var streamWriter = new StreamWriter(stream))
                    {
                        var num = 1;
                        foreach (var keyValuePair in _fileNameIndex)
                        {
                            var fileName = Path.GetFileName(keyValuePair.Value);
                            streamWriter.Write(keyValuePair.Key);
                            streamWriter.Write(char.MinValue);
                            streamWriter.Write(fileName);
                            if (num < _fileNameIndex.Count)
                                streamWriter.Write(char.MinValue);
                            ++num;
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                //this.RaiseExceptionFilter((Exception)ex, "Failed to save window configuration index file.");
            }
            catch (UnauthorizedAccessException ex)
            {
                //this.RaiseExceptionFilter((Exception)ex, "Failed to save window configuration index file.");
            }
        }


        private string GetFileNameIndexFullPath()
        {
            var stringBuilder = new StringBuilder(ProfileRootDirectory);
            stringBuilder.Append(Path.DirectorySeparatorChar);
            stringBuilder.Append("Windows");
            stringBuilder.Append(".index");
            return stringBuilder.ToString();
        }


    }
}
