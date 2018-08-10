using System.IO;
using System.Text;
using System.Threading;
using ModernApplicationFramework.Text.NativeMethods;

namespace ModernApplicationFramework.Text.Data
{
    public class FileUtilities
    {
        public static void SaveSnapshot(ITextSnapshot snapshot, FileMode fileMode, Encoding encoding, string filePath)
        {
            string temporaryPath = null;
            try
            {
                var fileStream1 = CreateFileStream(filePath, fileMode, out temporaryPath, out var originalFileStream);
                if (originalFileStream == null)
                {
                    try
                    {
                        using (var streamWriter = new StreamWriter(fileStream1, encoding))
                            snapshot.Write(streamWriter);
                    }
                    finally
                    {
                        fileStream1.Dispose();
                    }
                    if (temporaryPath == null)
                        return;
                    var num = 3;
                    do
                    {
                        try
                        {
                            File.Replace(temporaryPath, filePath, null, true);
                            temporaryPath = null;
                            return;
                        }
                        catch (FileNotFoundException)
                        {
                            File.Move(temporaryPath, filePath);
                            temporaryPath = null;
                            return;
                        }
                        catch (IOException)
                        {
                            Thread.Sleep(5);
                        }
                    }
                    while (--num > 0);
                    using (var fileStream3 = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read))
                    {
                        using (var streamWriter = new StreamWriter(fileStream3, encoding))
                            snapshot.Write(streamWriter);
                    }
                }
                else
                {
                    try
                    {
                        originalFileStream.CopyTo(fileStream1);
                        try
                        {
                            originalFileStream.Seek(0L, SeekOrigin.Begin);
                            originalFileStream.SetLength(0L);
                            using (var streamWriter = new StreamWriter(originalFileStream, encoding, 1024, true))
                                snapshot.Write(streamWriter);
                        }
                        catch
                        {
                            fileStream1.Seek(0L, SeekOrigin.Begin);
                            originalFileStream.Seek(0L, SeekOrigin.Begin);
                            originalFileStream.SetLength(0L);
                            fileStream1.CopyTo(originalFileStream);
                            throw;
                        }
                    }
                    finally
                    {
                        originalFileStream.Dispose();
                        fileStream1.Dispose();
                    }
                }
            }
            finally
            {
                if (temporaryPath != null)
                {
                    try
                    {
                        if (File.Exists(temporaryPath))
                            File.Delete(temporaryPath);
                    }
                    catch
                    {
                    }
                }
            }
        }

        private static FileStream CreateFileStream(string filePath, FileMode fileMode, out string temporaryPath, out FileStream originalFileStream)
        {
            originalFileStream = null;
            if (File.Exists(filePath))
            {
                if (fileMode == FileMode.CreateNew)
                    throw new IOException(filePath + " exists");
                try
                {
                    originalFileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                    var safeFileHandle = originalFileStream.SafeFileHandle;
                    if (!safeFileHandle.IsClosed)
                    {
                        if (!safeFileHandle.IsInvalid)
                        {
                            if (Kernel32.GetFileInformationByHandle(safeFileHandle, out var lpFileInformation))
                            {
                                if (lpFileInformation.NumberOfLinks <= 1U)
                                {
                                    originalFileStream.Dispose();
                                    originalFileStream = null;
                                }
                            }
                        }
                    }
                }
                catch
                {
                    if (originalFileStream == null)
                        throw;
                    originalFileStream.Dispose();
                    originalFileStream = null;
                    throw;
                }
                var directoryName = Path.GetDirectoryName(filePath);
                var num = 0;
                while (++num < 20)
                {
                    try
                    {
                        temporaryPath = Path.Combine(directoryName, Path.GetRandomFileName() + "~");
                        return new FileStream(temporaryPath, FileMode.CreateNew, originalFileStream != null ? FileAccess.ReadWrite : FileAccess.Write, FileShare.None);
                    }
                    catch (IOException)
                    {
                    }
                }
            }
            temporaryPath = null;
            return new FileStream(filePath, fileMode, FileAccess.Write, FileShare.Read);
        }       
    }
}