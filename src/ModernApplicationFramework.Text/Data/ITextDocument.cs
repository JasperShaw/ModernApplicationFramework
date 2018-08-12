using System;
using System.Text;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Text.Data
{
    public interface ITextDocument : IDisposable
    {
        string FilePath { get; }

        ITextBuffer TextBuffer { get; }

        bool IsDirty { get; }

        DateTime LastSavedTime { get; }

        DateTime LastContentModifiedTime { get; }

        Encoding Encoding { get; set; }

        void SetEncoderFallback(EncoderFallback fallback);

        event EventHandler<EncodingChangedEventArgs> EncodingChanged;

        event EventHandler<TextDocumentFileActionEventArgs> FileActionOccurred;

        event EventHandler DirtyStateChanged;

        void Rename(string newFilePath);

        ReloadResult Reload();

        ReloadResult Reload(EditOptions options);

        bool IsReloading { get; }

        void Save();

        void SaveAs(string filePath, bool overwrite);

        void SaveAs(string filePath, bool overwrite, bool createFolder);

        void SaveAs(string filePath, bool overwrite, IContentType newContentType);

        void SaveAs(string filePath, bool overwrite, bool createFolder, IContentType newContentType);

        void SaveCopy(string filePath, bool overwrite);

        void SaveCopy(string filePath, bool overwrite, bool createFolder);

        void UpdateDirtyState(bool isDirty, DateTime lastContentModifiedTime);
    }
}