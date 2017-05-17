using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Xml;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Basics.SettingsManager
{
    [Export(typeof(ISettingsManager))]
    public class SettingsManager : ISettingsManager
    {
        private readonly object _lockObject = new object();

        protected SettingsFileDeserializer Deserializer;

        public IEnvironmentVarirables EnvironmentVarirables { get; }

        public XmlDocument SettingsFile { get; protected set; }

        [ImportingConstructor]
        public SettingsManager(IEnvironmentVarirables environmentVarirables)
        {
            EnvironmentVarirables = environmentVarirables;
            Initialized += SettingsManager_Initialized;
            Deserializer = new SettingsFileDeserializer();
        }

        public void ChangeSettingsFileLocation(string path, bool deleteCurrent)
        {
            lock (_lockObject)
            {
                if (!File.Exists(EnvironmentVarirables.SettingsFilePath))
                    throw new FileNotFoundException(EnvironmentVarirables.SettingsFilePath);

                var newDirectoryPath = Path.GetDirectoryName(path);

                if (string.IsNullOrEmpty(newDirectoryPath))
                    throw new ArgumentNullException();
                if (!Directory.Exists(newDirectoryPath))
                    throw new DirectoryNotFoundException();

                File.Copy(EnvironmentVarirables.SettingsFilePath, path);

                if (deleteCurrent)
                    File.Delete(EnvironmentVarirables.SettingsFilePath);
                EnvironmentVarirables.SettingsFilePath = path;
            }
            SettingsLocationChanged?.Invoke(this, EventArgs.Empty);
        }

        public virtual void CreateNewSettingsFile()
        {
            CreateNewSettingsFileInternal(EnvironmentVarirables.SettingsFilePath);
        }

        public void DeleteCurrentSettingsFile()
        {
            if (File.Exists(EnvironmentVarirables.SettingsFilePath))
                File.Delete(EnvironmentVarirables.SettingsFilePath);
        }

        public void Initialize()
        {
            if (!File.Exists(EnvironmentVarirables.SettingsFilePath))
                CreateNewSettingsFile();
            LoadCurrent();
            Initialized?.Invoke(this, EventArgs.Empty);

            TryGetValue("UserSettings/ApplicationIdentity/@version", out string value);
        }

        public virtual void LoadCurrent()
        {
            SettingsFile = Deserializer.Desrialize(EnvironmentVarirables.SettingsFilePath); 
        }

        protected void CreateNewSettingsFileInternal(string path)
        {
            var serializer = new SettingsFileSerializer(this);
            serializer.Serialize(path);
        }

        private void SettingsManager_Initialized(object sender, EventArgs e)
        {
            Initialized -= SettingsManager_Initialized;
            //TODO: Read real settingspath from file and update if neccessary
        }

        public event EventHandler SettingsLocationChanged;
        public event EventHandler Initialized;


        public GetValueResult TryGetValue<T>(string name, out T value)
        {


            return Deserializer.Deserialize<T>(name, out value);

            //value = CastWithUnboxing<T>(SettingsFile.SelectSingleNode(name)?.Value);
            //return GetValueResult.Success;
        }

        public Task SetValueAsync(string name, object value)
        {
            var t = new Task(() =>
            {
                
            });
            t.Start();
            return t;
        }



        private static T CastWithUnboxing<T>(object value)
        {
            if (typeof(T).IsEnum)
                return (T)Enum.ToObject(typeof(T), value);
            if (typeof(T) == typeof(bool) && !(value is bool))
            {
                object boxed = value;
                if (TryReboxAnyIntegralTypeAs<ulong>(ref boxed))
                    return (T) (object) ((ulong) boxed > 0UL);
            }
            if (value is bool && typeof(T) != typeof(bool))
            {
                object boxed = (bool)value ? 1 : 0;
                if (TryReboxInt32As<T>(ref boxed))
                    value = boxed;
            }
            var flag = TryReboxAnyIntegralTypeAs<T>(ref value) || TryReboxFloatAs<T>(ref value) || TryReboxDoubleAs<T>(ref value) || TryReboxDecimalAs<T>(ref value);
            return (T)value;
        }

        private static bool TryReboxAnyIntegralTypeAs<T>(ref object boxed)
        {
            if (!TryReboxByteAs<T>(ref boxed) && !TryReboxSbyteAs<T>(ref boxed) && !TryReboxInt16As<T>(ref boxed) &&
                !TryReboxUint16As<T>(ref boxed) && (!TryReboxInt32As<T>(ref boxed) && !TryReboxUint32As<T>(ref boxed) &&
                                                    !TryReboxInt64As<T>(ref boxed)))
                return TryReboxUint64As<T>(ref boxed);
            return true;
        }

        private static bool TryReboxByteAs<T>(ref object boxed)
        {
            if (!(boxed is byte))
                return false;
            byte num = (byte)boxed;
            if (typeof(T) == typeof(byte))
                boxed = num;
            else if (typeof(T) == typeof(sbyte))
                boxed = (sbyte)num;
            else if (typeof(T) == typeof(short))
                boxed = (short)num;
            else if (typeof(T) == typeof(ushort))
                boxed = (ushort)num;
            else if (typeof(T) == typeof(int))
                boxed = (int)num;
            else if (typeof(T) == typeof(uint))
                boxed = (uint)num;
            else if (typeof(T) == typeof(long))
            {
                boxed = (long)num;
            }
            else
            {
                if (!(typeof(T) == typeof(ulong)))
                    return false;
                boxed = (ulong)num;
            }
            return true;
        }

        private static bool TryReboxSbyteAs<T>(ref object boxed)
        {
            if (!(boxed is sbyte))
                return false;
            sbyte num = (sbyte)boxed;
            if (typeof(T) == typeof(byte))
                boxed = (byte)num;
            else if (typeof(T) == typeof(sbyte))
                boxed = num;
            else if (typeof(T) == typeof(short))
                boxed = (short)num;
            else if (typeof(T) == typeof(ushort))
                boxed = (ushort)num;
            else if (typeof(T) == typeof(int))
                boxed = (int)num;
            else if (typeof(T) == typeof(uint))
                boxed = (uint)num;
            else if (typeof(T) == typeof(long))
            {
                boxed = (long)num;
            }
            else
            {
                if (!(typeof(T) == typeof(ulong)))
                    return false;
                boxed = (ulong)num;
            }
            return true;
        }

        private static bool TryReboxInt16As<T>(ref object boxed)
        {
            if (!(boxed is short))
                return false;
            short num = (short)boxed;
            if (typeof(T) == typeof(byte))
                boxed = (byte)num;
            else if (typeof(T) == typeof(sbyte))
                boxed = (sbyte)num;
            else if (typeof(T) == typeof(short))
                boxed = num;
            else if (typeof(T) == typeof(ushort))
                boxed = (ushort)num;
            else if (typeof(T) == typeof(int))
                boxed = (int)num;
            else if (typeof(T) == typeof(uint))
                boxed = (uint)num;
            else if (typeof(T) == typeof(long))
            {
                boxed = (long)num;
            }
            else
            {
                if (!(typeof(T) == typeof(ulong)))
                    return false;
                boxed = (ulong)num;
            }
            return true;
        }

        private static bool TryReboxUint16As<T>(ref object boxed)
        {
            if (!(boxed is ushort))
                return false;
            ushort num = (ushort)boxed;
            if (typeof(T) == typeof(byte))
                boxed = (byte)num;
            else if (typeof(T) == typeof(sbyte))
                boxed = (sbyte)num;
            else if (typeof(T) == typeof(short))
                boxed = (short)num;
            else if (typeof(T) == typeof(ushort))
                boxed = num;
            else if (typeof(T) == typeof(int))
                boxed = (int)num;
            else if (typeof(T) == typeof(uint))
                boxed = (uint)num;
            else if (typeof(T) == typeof(long))
            {
                boxed = (long)num;
            }
            else
            {
                if (!(typeof(T) == typeof(ulong)))
                    return false;
                boxed = (ulong)num;
            }
            return true;
        }

        private static bool TryReboxInt32As<T>(ref object boxed)
        {
            if (!(boxed is int))
                return false;
            int num = (int)boxed;
            if (typeof(T) == typeof(byte))
                boxed = (byte)num;
            else if (typeof(T) == typeof(sbyte))
                boxed = (sbyte)num;
            else if (typeof(T) == typeof(short))
                boxed = (short)num;
            else if (typeof(T) == typeof(ushort))
                boxed = (ushort)num;
            else if (typeof(T) == typeof(int))
                boxed = num;
            else if (typeof(T) == typeof(uint))
                boxed = (uint)num;
            else if (typeof(T) == typeof(long))
            {
                boxed = (long)num;
            }
            else
            {
                if (!(typeof(T) == typeof(ulong)))
                    return false;
                boxed = (ulong)num;
            }
            return true;
        }

        private static bool TryReboxUint32As<T>(ref object boxed)
        {
            if (!(boxed is uint))
                return false;
            uint num = (uint)boxed;
            if (typeof(T) == typeof(byte))
                boxed = (byte)num;
            else if (typeof(T) == typeof(sbyte))
                boxed = (sbyte)num;
            else if (typeof(T) == typeof(short))
                boxed = (short)num;
            else if (typeof(T) == typeof(ushort))
                boxed = (ushort)num;
            else if (typeof(T) == typeof(int))
                boxed = (int)num;
            else if (typeof(T) == typeof(uint))
                boxed = num;
            else if (typeof(T) == typeof(long))
            {
                boxed = (long)num;
            }
            else
            {
                if (!(typeof(T) == typeof(ulong)))
                    return false;
                boxed = (ulong)num;
            }
            return true;
        }

        private static bool TryReboxInt64As<T>(ref object boxed)
        {
            if (!(boxed is long))
                return false;
            long num = (long)boxed;
            if (typeof(T) == typeof(byte))
                boxed = (byte)num;
            else if (typeof(T) == typeof(sbyte))
                boxed = (sbyte)num;
            else if (typeof(T) == typeof(short))
                boxed = (short)num;
            else if (typeof(T) == typeof(ushort))
                boxed = (ushort)num;
            else if (typeof(T) == typeof(int))
                boxed = (int)num;
            else if (typeof(T) == typeof(uint))
                boxed = (uint)num;
            else if (typeof(T) == typeof(long))
            {
                boxed = num;
            }
            else
            {
                if (!(typeof(T) == typeof(ulong)))
                    return false;
                boxed = (ulong)num;
            }
            return true;
        }

        private static bool TryReboxUint64As<T>(ref object boxed)
        {
            if (!(boxed is ulong))
                return false;
            ulong num = (ulong)boxed;
            if (typeof(T) == typeof(byte))
                boxed = (byte)num;
            else if (typeof(T) == typeof(sbyte))
                boxed = (sbyte)num;
            else if (typeof(T) == typeof(short))
                boxed = (short)num;
            else if (typeof(T) == typeof(ushort))
                boxed = (ushort)num;
            else if (typeof(T) == typeof(int))
                boxed = (int)num;
            else if (typeof(T) == typeof(uint))
                boxed = (uint)num;
            else if (typeof(T) == typeof(long))
            {
                boxed = (long)num;
            }
            else
            {
                if (!(typeof(T) == typeof(ulong)))
                    return false;
                boxed = num;
            }
            return true;
        }

        private static bool TryReboxFloatAs<T>(ref object boxed)
        {
            if (!(boxed is float))
                return false;
            float num = (float)boxed;
            if (typeof(T) == typeof(float))
                boxed = num;
            else if (typeof(T) == typeof(double))
            {
                boxed = (double)num;
            }
            else
            {
                if (!(typeof(T) == typeof(Decimal)))
                    return false;
                boxed = (Decimal)num;
            }
            return true;
        }

        private static bool TryReboxDoubleAs<T>(ref object boxed)
        {
            if (!(boxed is double))
                return false;
            double num = (double)boxed;
            if (typeof(T) == typeof(float))
                boxed = (float)num;
            else if (typeof(T) == typeof(double))
            {
                boxed = num;
            }
            else
            {
                if (!(typeof(T) == typeof(Decimal)))
                    return false;
                boxed = (Decimal)num;
            }
            return true;
        }

        private static bool TryReboxDecimalAs<T>(ref object boxed)
        {
            if (!(boxed is Decimal))
                return false;
            Decimal num = (Decimal)boxed;
            if (typeof(T) == typeof(float))
                boxed = (float)num;
            else if (typeof(T) == typeof(double))
            {
                boxed = (double)num;
            }
            else
            {
                if (!(typeof(T) == typeof(Decimal)))
                    return false;
                boxed = num;
            }
            return true;
        }



    }

    public class SettingsManagerException : Exception
    {
        public SettingsManagerException()
        {
        }

        public SettingsManagerException(string message) : base(message)
        {
        }

        public SettingsManagerException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SettingsManagerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    public interface ISettingsManager
    {

        IEnvironmentVarirables EnvironmentVarirables { get; }

        void ChangeSettingsFileLocation(string path, bool deleteCurrent);

        void CreateNewSettingsFile();

        void DeleteCurrentSettingsFile();

        void Initialize();

        void LoadCurrent();
        event EventHandler SettingsLocationChanged;
        event EventHandler Initialized;

        //ISettingsSubset GetSubset(string namePattern);

        //string[] NamesStartingWith(string prefix);

        //ISettingsList GetOrCreateList(string name, bool isMachineLocal);

        GetValueResult TryGetValue<T>(string name, out T value);

        //T GetValueOrDefault<T>(string name, T defaultValue);

        Task SetValueAsync(string name, object value);
    }

    public enum GetValueResult
    {
        Success,
        Missing,
        Corrupt,
        IncompatibleType,
        ObsoleteFormat,
        UnknownError,
    }
}

