using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ModernApplicationFramework.Utilities;
using ModernApplicationFramework.WindowManagement.LayoutState;

namespace ModernApplicationFramework.WindowManagement.WindowProfile
{
    internal class WindowProfileManager
    {
        public readonly ICollection<WindowProfile> Profiles = new List<WindowProfile>();
        private readonly Dictionary<string, string> _fileNameIndex = new Dictionary<string, string>();
        private string _fileNameIndexFullPath;


        private bool _isFileNameIndexLoaded;


        public WindowProfileManager(string profileRootDirectory)
        {
            Validate.IsNotNull(profileRootDirectory, nameof(profileRootDirectory));
            if (!Path.IsPathRooted(profileRootDirectory))
                throw new ArgumentException("Profile root directory must be a rooted path",
                    nameof(profileRootDirectory));
            ProfileRootDirectory = profileRootDirectory;
        }


        public event EventHandler<WindowProfileEventArgs> ProfileAdded;

        public event EventHandler<WindowProfileEventArgs> ProfileSet;

        private string FileNameIndexFullPath =>
            _fileNameIndexFullPath ?? (_fileNameIndexFullPath = GetFileNameIndexFullPath());


        private string ProfileRootDirectory { get; }

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

        public WindowProfile CreateProfile(string profileName, Func<string, WindowProfile> activator)
        {
            Validate.IsNotNullAndNotEmpty(profileName, nameof(profileName));
            EnsureFileNameIndexLoaded();
            var windowProfile = activator(profileName) ?? WindowProfile.Create(profileName);
            AddProfile(windowProfile);
            return windowProfile;
        }

        public WindowProfile GetOrCreateProfile(string profileName, Func<string, WindowProfile> activator)
        {
            Validate.IsNotNullAndNotEmpty(profileName, nameof(profileName));
            Validate.IsNotNull(activator, nameof(activator));
            return GetProfile(profileName) ?? CreateProfile(profileName, activator);
        }


        public WindowProfile GetProfile(string profileName, bool fromBackup = false)
        {
            Validate.IsNotNullAndNotEmpty(profileName, nameof(profileName));
            var profile = default(WindowProfile);
            if ((fromBackup || !TryGetProfile(profileName, out profile)) && IsProfileInIndex(profileName))
                profile = LoadProfileFromLocalStorage(profileName, fromBackup);
            if (profile != null)
                AddProfile(profile);
            return profile;
        }

        public bool IsProfileInIndex(string profileName)
        {
            Validate.IsNotNullAndNotEmpty(profileName, nameof(profileName));
            EnsureFileNameIndexLoaded();
            return _fileNameIndex.ContainsKey(profileName);
        }

        public Stream OpenProfileLocalStorage(string profileName, FileAccess fileAccess)
        {
            Validate.IsNotNullAndNotEmpty(profileName, nameof(profileName));
            var mode = fileAccess == FileAccess.Read ? FileMode.Open : FileMode.Create;
            var share = fileAccess == FileAccess.Read ? FileShare.Read : FileShare.None;
            return File.Open(GetProfileFullPath(profileName), mode, fileAccess, share);
        }

        public void Save(WindowProfile profile)
        {
            Validate.IsNotNull(profile, nameof(profile));
            if (!EnsureLocalStorageDirectoryExists())
                return;
            SaveProfileToLocalStorage(profile);
        }

        internal void Backup(string profileName)
        {
            try
            {
                if (!IsProfileInIndex(profileName))
                    return;
                if (TryGetProfile(profileName, out var profile))
                    Save(profile);
                using (var stream = OpenProfileLocalStorage(profileName, FileAccess.Read))
                    using (var destination = OpenProfileBackupLocalStorage(profileName, FileAccess.Write))
                        stream.CopyTo(destination);
            }
            catch (IOException ex)
            {
                //this.RaiseExceptionFilter((Exception)ex, "Failed to create a backup of window configuration file.");
            }
            catch (UnauthorizedAccessException ex)
            {
                //this.RaiseExceptionFilter((Exception)ex, "Failed to create a backup of window configuration file.");
            }
        }

        public bool Reload(string profileName)
        {
            Validate.IsNotNullAndNotEmpty(profileName, nameof(profileName));
            if (!IsProfileInIndex(profileName))
                return false;
            var windowProfile = LoadProfileFromLocalStorage(profileName);
            if (windowProfile == null)
                return false;
            AddProfile(windowProfile);
            return true;
        }

        internal void DeleteProfileBackupFiles()
        {
            try
            {
                foreach (string enumerateFile in Directory.EnumerateFiles(ProfileRootDirectory, "*.winprf_backup"))
                    File.Delete(enumerateFile);
            }
            catch (IOException ex)
            {
                //this.RaiseExceptionFilter((Exception)ex, "Failed to delete window configuration backup file.");
            }
            catch (UnauthorizedAccessException ex)
            {
                //this.RaiseExceptionFilter((Exception)ex, "Failed to delete window configuration backup file.");
            }
        }

        internal void RestoreProfilesFromBackup()
        {
            try
            {
                foreach (string enumerateFile in Directory.EnumerateFiles(ProfileRootDirectory, "*.winprf_backup"))
                {
                    string withoutExtension = Path.GetFileNameWithoutExtension(enumerateFile);
                    using (Stream destination = OpenProfileLocalStorage(withoutExtension, FileAccess.Write))
                    {
                        using (Stream stream = File.Open(enumerateFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                            stream.CopyTo(destination);
                    }
                    if (TryGetProfile(withoutExtension, out var profile))
                        Reload(withoutExtension);
                }
            }
            catch (IOException ex)
            {
                //this.RaiseExceptionFilter((Exception)ex, "Failed to restor a window configuration file from backup.");
            }
            catch (UnauthorizedAccessException ex)
            {
                //this.RaiseExceptionFilter((Exception)ex, "Failed to restor a window configuration file from backup.");
            }
        }

        protected void OnProfileAdded(WindowProfile newProfile)
        {
            ProfileAdded?.Invoke(this, new WindowProfileEventArgs(newProfile));
        }


        protected void OnProfileSet(WindowProfile newProfile)
        {
            ProfileSet?.Invoke(this, new WindowProfileEventArgs(newProfile));
        }

        private void EnsureFileNameIndexLoaded()
        {
            if (_isFileNameIndexLoaded)
                return;
            LoadFileNameIndexFromLocalStorage();
            _isFileNameIndexLoaded = true;
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
            var stringBuilder = new StringBuilder(profileName.Length > 50 ? profileName.Substring(0, 50) : profileName);
            foreach (var invalidFileNameChar in Path.GetInvalidFileNameChars())
                stringBuilder.Replace(invalidFileNameChar, '_');
            stringBuilder.Append("_");
            stringBuilder.Append(Path.GetRandomFileName());
            stringBuilder.Append(".winprf");
            return Path.Combine(ProfileRootDirectory, stringBuilder.ToString());
        }


        private string GetFileNameIndexFullPath()
        {
            var stringBuilder = new StringBuilder(ProfileRootDirectory);
            stringBuilder.Append(Path.DirectorySeparatorChar);
            stringBuilder.Append("Windows");
            stringBuilder.Append(".index");
            return stringBuilder.ToString();
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

        private string GetProfileBackupFullPath(string profileName)
        {
            var stringBuilder = new StringBuilder(profileName);
            stringBuilder.Append(".winprf_backup");
            return Path.Combine(ProfileRootDirectory, stringBuilder.ToString());
        }

        private string GetProfileFullPath(string profileName)
        {
            EnsureFileNameIndexLoaded();
            return GetOrCreateProfileFileName(profileName);
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
                            _fileNameIndex[strArray[index]] = Path.Combine(ProfileRootDirectory,
                                Path.GetFileName(strArray[index + 1]));
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

        private WindowProfile LoadProfileFromLocalStorage(string profileName, bool fromBackup = false)
        {
            try
            {
                var path = fromBackup ? GetProfileBackupFullPath(profileName) : GetProfileFullPath(profileName);
                var payload = LayoutPayloadUtilities.FileToPayloadData(path);
                return new WindowProfile(profileName, payload);
            }
            catch
            {
                return null;
            }
        }

        private Stream OpenProfileBackupLocalStorage(string profileName, FileAccess fileAccess)
        {
            Validate.IsNotNullAndNotEmpty(profileName, nameof(profileName));
            var mode = fileAccess == FileAccess.Read ? FileMode.Open : FileMode.Create;
            var share = fileAccess == FileAccess.Read ? FileShare.Read : FileShare.None;
            return File.Open(GetProfileBackupFullPath(profileName), mode, fileAccess, share);
        }

        private void SaveFileNameIndex()
        {
            if (!EnsureLocalStorageDirectoryExists())
                return;
            SaveFileNameIndexToLocalStorage();
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


        private void SaveProfileToLocalStorage(WindowProfile profile)
        {
            try
            {
                LayoutPayloadUtilities.PayloadDataToFile(GetProfileFullPath(profile.Name), profile.StatePlayload);
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

        private bool TryGetProfile(string profileName, out WindowProfile profile)
        {
            profile = null;
            if (Profiles.All(x => x.Name != profileName))
                return false;
            profile = Profiles.First(x => x.Name == profileName);
            return true;
        }
    }
}