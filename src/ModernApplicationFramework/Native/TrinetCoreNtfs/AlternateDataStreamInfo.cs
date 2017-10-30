using System;
using System.ComponentModel;
using System.IO;
using System.Security.Permissions;
using ModernApplicationFramework.Native.NativeMethods;
using ModernApplicationFramework.Native.Platform.Enums;
using ModernApplicationFramework.Native.Platform.Structs;

namespace ModernApplicationFramework.Native.TrinetCoreNtfs
{
    /// <summary>
	/// Represents the details of an alternative data stream.
	/// </summary>
    public sealed class AlternateDataStreamInfo : IEquatable<AlternateDataStreamInfo>
    {
        /// <summary>
		/// Initializes a new instance of the <see cref="AlternateDataStreamInfo"/> class.
		/// </summary>
		/// <param name="filePath">
		/// The full path of the file.
		/// This argument must not be <see langword="null"/>.
		/// </param>
		/// <param name="info">
		/// The <see cref="Win32StreamInfo"/> containing the stream information.
		/// </param>
		internal AlternateDataStreamInfo(string filePath, Win32StreamInfo info)
        {
            FilePath = filePath;
            Name = info.StreamName;
            StreamType = info.StreamType;
            Attributes = info.StreamAttributes;
            Size = info.StreamSize;
            Exists = true;

            FullPath = NativeMethods.NativeMethods.BuildStreamPath(FilePath, Name);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AlternateDataStreamInfo"/> class.
        /// </summary>
        /// <param name="filePath">
        /// The full path of the file.
        /// This argument must not be <see langword="null"/>.
        /// </param>
        /// <param name="streamName">
        /// The name of the stream
        /// This argument must not be <see langword="null"/>.
        /// </param>
        /// <param name="fullPath">
        /// The full path of the stream.
        /// If this argument is <see langword="null"/>, it will be generated from the
        /// <paramref name="filePath"/> and <paramref name="streamName"/> arguments.
        /// </param>
        /// <param name="exists">
        /// <see langword="true"/> if the stream exists;
        /// otherwise, <see langword="false"/>.
        /// </param>
        internal AlternateDataStreamInfo(string filePath, string streamName, string fullPath, bool exists)
        {
            if (string.IsNullOrEmpty(fullPath)) fullPath = NativeMethods.NativeMethods.BuildStreamPath(filePath, streamName);
            StreamType = FileStreamType.AlternateDataStream;

            FilePath = filePath;
            Name = streamName;
            FullPath = fullPath;
            Exists = exists;

            if (Exists)
            {
                Size = NativeMethods.NativeMethods.GetFileSize(FullPath);
            }
        }

        /// <summary>
		/// Returns the full path of this stream.
		/// </summary>
		/// <value>
		/// The full path of this stream.
		/// </value>
		public string FullPath { get; }

        /// <summary>
        /// Returns the full path of the file which contains the stream.
        /// </summary>
        /// <value>
        /// The full file-system path of the file which contains the stream.
        /// </value>
        public string FilePath { get; }

        /// <summary>
        /// Returns the name of the stream.
        /// </summary>
        /// <value>
        /// The name of the stream.
        /// </value>
        public string Name { get; }

        /// <summary>
        /// Returns a flag indicating whether the specified stream exists.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if the stream exists;
        /// otherwise, <see langword="false"/>.
        /// </value>
        public bool Exists { get; }

        /// <summary>
        /// Returns the size of the stream, in bytes.
        /// </summary>
        /// <value>
        /// The size of the stream, in bytes.
        /// </value>
        public long Size { get; }

        /// <summary>
        /// Returns the type of data.
        /// </summary>
        /// <value>
        /// One of the <see cref="FileStreamType"/> values.
        /// </value>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public FileStreamType StreamType { get; }

        /// <summary>
        /// Returns attributes of the data stream.
        /// </summary>
        /// <value>
        /// A combination of <see cref="FileStreamAttributes"/> values.
        /// </value>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public FileStreamAttributes Attributes { get; }

        /// <summary>
		/// Returns a <see cref="string"/> that represents the current instance.
		/// </summary>
		/// <returns>
		/// A <see cref="string"/> that represents the current instance.
		/// </returns>
		public override string ToString()
        {
            return FullPath;
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            var comparer = StringComparer.OrdinalIgnoreCase;
            return comparer.GetHashCode(FilePath ?? string.Empty)
                ^ comparer.GetHashCode(Name ?? string.Empty);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="obj">
        /// An object to compare with this object.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the current object is equal to the <paramref name="obj"/> parameter;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;

            var other = obj as AlternateDataStreamInfo;
            return !ReferenceEquals(null, other) && Equals(other);
        }

        /// <summary>
        /// Returns a value indicating whether
        /// this instance is equal to another instance.
        /// </summary>
        /// <param name="other">
        /// The instance to compare to.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the current object is equal to the <paramref name="other"/> parameter;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public bool Equals(AlternateDataStreamInfo other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            var comparer = StringComparer.OrdinalIgnoreCase;
            return comparer.Equals(FilePath ?? string.Empty, other.FilePath ?? string.Empty)
                && comparer.Equals(Name ?? string.Empty, other.Name ?? string.Empty);
        }

        /// <summary>
        /// The equality operator.
        /// </summary>
        /// <param name="first">
        /// The first object.
        /// </param>
        /// <param name="second">
        /// The second object.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the two objects are equal;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator ==(AlternateDataStreamInfo first, AlternateDataStreamInfo second)
        {
            if (ReferenceEquals(first, second))
                return true;
            if (ReferenceEquals(null, first))
                return false;
            return !ReferenceEquals(null, second) && first.Equals(second);
        }

        /// <summary>
        /// The inequality operator.
        /// </summary>
        /// <param name="first">
        /// The first object.
        /// </param>
        /// <param name="second">
        /// The second object.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the two objects are not equal;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator !=(AlternateDataStreamInfo first, AlternateDataStreamInfo second)
        {
            if (ReferenceEquals(first, second))
                return false;
            if (ReferenceEquals(null, first))
                return true;
            if (ReferenceEquals(null, second))
                return true;
            return !first.Equals(second);
        }

        /// <summary>
		/// Deletes this stream from the parent file.
		/// </summary>
		/// <returns>
		/// <see langword="true"/> if the stream was deleted;
		/// otherwise, <see langword="false"/>.
		/// </returns>
		/// <exception cref="SecurityException">
		/// The caller does not have the required permission.
		/// </exception>
		/// <exception cref="UnauthorizedAccessException">
		/// The caller does not have the required permission, or the file is read-only.
		/// </exception>
		/// <exception cref="IOException">
		/// The specified file is in use.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// The path of the stream is invalid.
		/// </exception>
		public bool Delete()
        {
            const FileIOPermissionAccess permAccess = FileIOPermissionAccess.Write;
            new FileIOPermission(permAccess, FilePath).Demand();
            return NativeMethods.NativeMethods.SafeDeleteFile(FullPath);
        }

        /// <summary>
		/// Calculates the access to demand.
		/// </summary>
		/// <param name="mode">
		/// The <see cref="FileMode"/>.
		/// </param>
		/// <param name="access">
		/// The <see cref="FileAccess"/>.
		/// </param>
		/// <returns>
		/// The <see cref="FileIOPermissionAccess"/>.
		/// </returns>
		private static FileIOPermissionAccess CalculateAccess(FileMode mode, FileAccess access)
        {
            FileIOPermissionAccess permAccess = FileIOPermissionAccess.NoAccess;
            switch (mode)
            {
                case FileMode.Append:
                    permAccess = FileIOPermissionAccess.Append;
                    break;

                case FileMode.Create:
                case FileMode.CreateNew:
                case FileMode.OpenOrCreate:
                case FileMode.Truncate:
                    permAccess = FileIOPermissionAccess.Write;
                    break;

                case FileMode.Open:
                    permAccess = FileIOPermissionAccess.Read;
                    break;
            }
            switch (access)
            {
                case FileAccess.ReadWrite:
                    permAccess |= FileIOPermissionAccess.Write;
                    permAccess |= FileIOPermissionAccess.Read;
                    break;

                case FileAccess.Write:
                    permAccess |= FileIOPermissionAccess.Write;
                    break;

                case FileAccess.Read:
                    permAccess |= FileIOPermissionAccess.Read;
                    break;
            }

            return permAccess;
        }

        /// <summary>
        /// Opens this alternate data stream.
        /// </summary>
        /// <param name="mode">
        /// A <see cref="FileMode"/> value that specifies whether a stream is created if one does not exist,
        /// and determines whether the contents of existing streams are retained or overwritten.
        /// </param>
        /// <param name="access">
        /// A <see cref="FileAccess"/> value that specifies the operations that can be performed on the stream.
        /// </param>
        /// <param name="share">
        /// A <see cref="FileShare"/> value specifying the type of access other threads have to the file.
        /// </param>
        /// <param name="bufferSize">
        /// The size of the buffer to use.
        /// </param>
        /// <param name="useAsync">
        /// <see langword="true"/> to enable async-IO;
        /// otherwise, <see langword="false"/>.
        /// </param>
        /// <returns>
        /// A <see cref="FileStream"/> for this alternate data stream.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="bufferSize"/> is less than or equal to zero.
        /// </exception>
        /// <exception cref="SecurityException">
        /// The caller does not have the required permission.
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        /// The caller does not have the required permission, or the file is read-only.
        /// </exception>
        /// <exception cref="IOException">
        /// The specified file is in use.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The path of the stream is invalid.
        /// </exception>
        /// <exception cref="Win32Exception">
        /// There was an error opening the stream.
        /// </exception>
        public FileStream Open(FileMode mode, FileAccess access, FileShare share, int bufferSize, bool useAsync)
        {
            if (0 >= bufferSize) throw new ArgumentOutOfRangeException(nameof(bufferSize), bufferSize, null);

            var permAccess = CalculateAccess(mode, access);
            new FileIOPermission(permAccess, FilePath).Demand();

            var flags = useAsync ? NativeFileFlags.Overlapped : 0;
            var handle = NativeMethods.NativeMethods.SafeCreateFile(FullPath, access.ToNative(), share, IntPtr.Zero, mode, flags, IntPtr.Zero);
            if (handle.IsInvalid) NativeMethods.NativeMethods.ThrowLastIoError(FullPath);
            return new FileStream(handle, access, bufferSize, useAsync);
        }

        /// <summary>
        /// Opens this alternate data stream.
        /// </summary>
        /// <param name="mode">
        /// A <see cref="FileMode"/> value that specifies whether a stream is created if one does not exist,
        /// and determines whether the contents of existing streams are retained or overwritten.
        /// </param>
        /// <param name="access">
        /// A <see cref="FileAccess"/> value that specifies the operations that can be performed on the stream.
        /// </param>
        /// <param name="share">
        /// A <see cref="FileShare"/> value specifying the type of access other threads have to the file.
        /// </param>
        /// <param name="bufferSize">
        /// The size of the buffer to use.
        /// </param>
        /// <returns>
        /// A <see cref="FileStream"/> for this alternate data stream.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="bufferSize"/> is less than or equal to zero.
        /// </exception>
        /// <exception cref="SecurityException">
        /// The caller does not have the required permission.
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        /// The caller does not have the required permission, or the file is read-only.
        /// </exception>
        /// <exception cref="IOException">
        /// The specified file is in use.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The path of the stream is invalid.
        /// </exception>
        /// <exception cref="Win32Exception">
        /// There was an error opening the stream.
        /// </exception>
        public FileStream Open(FileMode mode, FileAccess access, FileShare share, int bufferSize)
        {
            return Open(mode, access, share, bufferSize, false);
        }

        /// <summary>
        /// Opens this alternate data stream.
        /// </summary>
        /// <param name="mode">
        /// A <see cref="FileMode"/> value that specifies whether a stream is created if one does not exist,
        /// and determines whether the contents of existing streams are retained or overwritten.
        /// </param>
        /// <param name="access">
        /// A <see cref="FileAccess"/> value that specifies the operations that can be performed on the stream.
        /// </param>
        /// <param name="share">
        /// A <see cref="FileShare"/> value specifying the type of access other threads have to the file.
        /// </param>
        /// <returns>
        /// A <see cref="FileStream"/> for this alternate data stream.
        /// </returns>
        /// <exception cref="SecurityException">
        /// The caller does not have the required permission.
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        /// The caller does not have the required permission, or the file is read-only.
        /// </exception>
        /// <exception cref="IOException">
        /// The specified file is in use.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The path of the stream is invalid.
        /// </exception>
        /// <exception cref="Win32Exception">
        /// There was an error opening the stream.
        /// </exception>
        public FileStream Open(FileMode mode, FileAccess access, FileShare share)
        {
            return Open(mode, access, share, NativeMethods.NativeMethods.DefaultBufferSize, false);
        }

        /// <summary>
        /// Opens this alternate data stream.
        /// </summary>
        /// <param name="mode">
        /// A <see cref="FileMode"/> value that specifies whether a stream is created if one does not exist,
        /// and determines whether the contents of existing streams are retained or overwritten.
        /// </param>
        /// <param name="access">
        /// A <see cref="FileAccess"/> value that specifies the operations that can be performed on the stream.
        /// </param>
        /// <returns>
        /// A <see cref="FileStream"/> for this alternate data stream.
        /// </returns>
        /// <exception cref="SecurityException">
        /// The caller does not have the required permission.
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        /// The caller does not have the required permission, or the file is read-only.
        /// </exception>
        /// <exception cref="IOException">
        /// The specified file is in use.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The path of the stream is invalid.
        /// </exception>
        /// <exception cref="Win32Exception">
        /// There was an error opening the stream.
        /// </exception>
        public FileStream Open(FileMode mode, FileAccess access)
        {
            return Open(mode, access, FileShare.None, NativeMethods.NativeMethods.DefaultBufferSize, false);
        }

        /// <summary>
        /// Opens this alternate data stream.
        /// </summary>
        /// <param name="mode">
        /// A <see cref="FileMode"/> value that specifies whether a stream is created if one does not exist,
        /// and determines whether the contents of existing streams are retained or overwritten.
        /// </param>
        /// <returns>
        /// A <see cref="FileStream"/> for this alternate data stream.
        /// </returns>
        /// <exception cref="SecurityException">
        /// The caller does not have the required permission.
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        /// The caller does not have the required permission, or the file is read-only.
        /// </exception>
        /// <exception cref="IOException">
        /// The specified file is in use.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The path of the stream is invalid.
        /// </exception>
        /// <exception cref="Win32Exception">
        /// There was an error opening the stream.
        /// </exception>
        public FileStream Open(FileMode mode)
        {
            FileAccess access = (FileMode.Append == mode) ? FileAccess.Write : FileAccess.ReadWrite;
            return Open(mode, access, FileShare.None, NativeMethods.NativeMethods.DefaultBufferSize, false);
        }

        /// <summary>
		/// Opens this stream for reading.
		/// </summary>
		/// <returns>
		/// A read-only <see cref="FileStream"/> for this stream.
		/// </returns>
		/// <exception cref="SecurityException">
		/// The caller does not have the required permission.
		/// </exception>
		/// <exception cref="UnauthorizedAccessException">
		/// The caller does not have the required permission, or the file is read-only.
		/// </exception>
		/// <exception cref="IOException">
		/// The specified file is in use.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// The path of the stream is invalid.
		/// </exception>
		/// <exception cref="Win32Exception">
		/// There was an error opening the stream.
		/// </exception>
		public FileStream OpenRead()
        {
            return Open(FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        /// <summary>
        /// Opens this stream for writing.
        /// </summary>
        /// <returns>
        /// A write-only <see cref="FileStream"/> for this stream.
        /// </returns>
        /// <exception cref="SecurityException">
        /// The caller does not have the required permission.
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        /// The caller does not have the required permission, or the file is read-only.
        /// </exception>
        /// <exception cref="IOException">
        /// The specified file is in use.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The path of the stream is invalid.
        /// </exception>
        /// <exception cref="Win32Exception">
        /// There was an error opening the stream.
        /// </exception>
        public FileStream OpenWrite()
        {
            return Open(FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
        }

        /// <summary>
        /// Opens this stream as a text file.
        /// </summary>
        /// <returns>
        /// A <see cref="StreamReader"/> which can be used to read the contents of this stream.
        /// </returns>
        /// <exception cref="SecurityException">
        /// The caller does not have the required permission.
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        /// The caller does not have the required permission, or the file is read-only.
        /// </exception>
        /// <exception cref="IOException">
        /// The specified file is in use.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The path of the stream is invalid.
        /// </exception>
        /// <exception cref="Win32Exception">
        /// There was an error opening the stream.
        /// </exception>
        public StreamReader OpenText()
        {
            Stream fileStream = Open(FileMode.Open, FileAccess.Read, FileShare.Read);
            return new StreamReader(fileStream);
        }
    }
}