using System;
using System.IO;
using System.Text;
using ModernApplicationFramework.TextEditor.Text;
using ModernApplicationFramework.TextEditor.Utilities;

namespace ModernApplicationFramework.TextEditor
{
    internal class TextDocument : ITextDocument
    {
        private readonly bool _attemptUtf8Detection;
        private int _cleanReiteratedVersion;
        private Encoding _encoding;
        private readonly bool _explicitEncoding;
        private bool _raisingDirtyStateChangedEvent;
        private bool _raisingFileActionChangedEvent;
        private readonly TextDocumentFactoryService _textDocumentFactoryService;
        public event EventHandler DirtyStateChanged;

        private DateTime _lastSavedTimeUtc;
        private DateTime _lastModifiedTimeUtc;

        public event EventHandler<EncodingChangedEventArgs> EncodingChanged;
        public event EventHandler<TextDocumentFileActionEventArgs> FileActionOccurred;
        public Encoding Encoding
        {
            get => _encoding;
            set
            {
                var encoding = _encoding;
                _encoding = value ?? throw new ArgumentNullException(nameof(value));
                if (_encoding.Equals(encoding))
                    return;
                _textDocumentFactoryService.GuardedOperations.RaiseEvent(this, EncodingChanged, new EncodingChangedEventArgs(encoding, _encoding));
            }
        }

        public string FilePath { get; private set; }

        public bool IsDirty { get; private set; }

        public bool IsReloading { get; private set; }

        public DateTime LastContentModifiedTime { get; private set; }

        public DateTime LastSavedTime => _lastSavedTimeUtc;

        public ITextBuffer TextBuffer { get; private set; }

        internal bool IsDisposed { get; private set; }

        internal TextDocument(ITextBuffer textBuffer, string filePath, DateTime lastModifiedTime,
            TextDocumentFactoryService textDocumentFactoryService)
            : this(textBuffer, filePath, lastModifiedTime, textDocumentFactoryService, Encoding.UTF8)
        {
        }

        internal TextDocument(ITextBuffer textBuffer, string filePath, DateTime lastModifiedTime,
            TextDocumentFactoryService textDocumentFactoryService, Encoding encoding, bool explicitEncoding = false,
            bool attemptUtf8Detection = true)
        {
            TextBuffer = textBuffer ?? throw new ArgumentNullException(nameof(textBuffer));
            FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            LastContentModifiedTime = lastModifiedTime;
            _textDocumentFactoryService = textDocumentFactoryService ??
                                          throw new ArgumentNullException(nameof(textDocumentFactoryService));
            _cleanReiteratedVersion = TextBuffer.CurrentSnapshot.Version.ReiteratedVersionNumber;
            IsDisposed = false;
            IsDirty = false;
            IsReloading = false;
            _raisingDirtyStateChangedEvent = false;
            _raisingFileActionChangedEvent = false;
            _encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));
            _explicitEncoding = explicitEncoding;
            _attemptUtf8Detection = attemptUtf8Detection;
            TextBuffer.ChangedHighPriority += TextBufferChangedHandler;
            TextBuffer.Properties.AddProperty(typeof(ITextDocument), this);
        }

        public void Dispose()
        {
            if (_raisingDirtyStateChangedEvent || _raisingFileActionChangedEvent)
                throw new InvalidOperationException();
            if (IsDisposed)
                return;
            TextBuffer.ChangedHighPriority -= TextBufferChangedHandler;
            TextBuffer.Properties.RemoveProperty(typeof(ITextDocument));
            IsDisposed = true;
            _textDocumentFactoryService.RaiseTextDocumentDisposed(this);
            GC.SuppressFinalize(this);
            TextBuffer = null;
        }

        public ReloadResult Reload()
        {
            return Reload(EditOptions.None);
        }

        public ReloadResult Reload(EditOptions options)
        {
            if (IsDisposed)
                throw new ObjectDisposedException("ITextDocument");
            if (_raisingDirtyStateChangedEvent || _raisingFileActionChangedEvent)
                throw new InvalidOperationException();
            var currentSnapshot = TextBuffer.CurrentSnapshot;
            var flag = false;
            Encoding encoding1;
            try
            {
                IsReloading = true;
                using (var stream = TextDocumentFactoryService.OpenFileGuts(FilePath, out _lastModifiedTimeUtc, out var fileSize))
                {
                    var encodingDetectorExtensions =
                        ExtensionSelector.SelectMatchingExtensions(_textDocumentFactoryService.OrderedEncodingDetectors,
                            TextBuffer.ContentType);
                    encoding1 = !_explicitEncoding ? EncodedStreamReader.DetectEncoding(stream, encodingDetectorExtensions, _textDocumentFactoryService.GuardedOperations) : Encoding;
                    if (encoding1 == null)
                    {
                        if (_attemptUtf8Detection)
                        {
                            try
                            {
                                var characterDetector = new ExtendedCharacterDetector();
                                ReloadBufferFromStream(stream, fileSize, options, characterDetector);
                                encoding1 = !characterDetector.DecodedExtendedCharacters
                                    ? Encoding.Default
                                    : new UTF8Encoding(false);
                            }
                            catch (DecoderFallbackException )
                            {
                                stream.Position = 0L;
                            }
                        }
                    }
                    if (encoding1 == null)
                        encoding1 = Encoding.Default;
                    if (currentSnapshot.Version.Next == null)
                    {
                        var fallback = new FallbackDetector(encoding1.DecoderFallback);
                        var encoding2 = (Encoding) encoding1.Clone();
                        encoding2.DecoderFallback = fallback;
                        ReloadBufferFromStream(stream, fileSize, options, encoding2);
                        if (fallback.FallbackOccurred)
                            flag = true;

                    }
                }
            }
            finally
            {
                IsReloading = false;
            }

            if (currentSnapshot.Version.Next == null)
                return ReloadResult.Aborted;
            _cleanReiteratedVersion = currentSnapshot.Version.Next.ReiteratedVersionNumber;
            RaiseFileActionChangedEvent(_lastModifiedTimeUtc, FileActionTypes.ContentLoadedFromDisk, FilePath);
            Encoding = encoding1;
            return !flag ? ReloadResult.Succeeded : ReloadResult.SucceededWithCharacterSubstitutions;
        }

        public void Rename(string newFilePath)
        {
            if (IsDisposed)
                throw new ObjectDisposedException("ITextDocument");
            if (_raisingDirtyStateChangedEvent || _raisingFileActionChangedEvent)
                throw new InvalidOperationException();
            FilePath = newFilePath ?? throw new ArgumentNullException(nameof(newFilePath));
            RaiseFileActionChangedEvent(_lastModifiedTimeUtc, FileActionTypes.DocumentRenamed, FilePath);
        }

        public void Save()
        {
            if (IsDisposed)
                throw new ObjectDisposedException("ITextDocument");
            if (_raisingDirtyStateChangedEvent || _raisingFileActionChangedEvent)
                throw new InvalidOperationException();
            if (TextBuffer.Properties.TryGetProperty("EncodingToBeAppliedOnSave", out Encoding property))
            {
                Encoding = property;
                TextBuffer.Properties.RemoveProperty("EncodingToBeAppliedOnSave");
            }

            PerformSave(FileMode.Create, FilePath, false);
            UpdateSaveStatus(FilePath, false);

        }

        public void SaveAs(string filePath, bool overwrite)
        {
            SaveAs(filePath, overwrite, false);
        }

        public void SaveAs(string filePath, bool overwrite, bool createFolder)
        {
            if (IsDisposed)
                throw new ObjectDisposedException("ITextDocument");
            if (_raisingDirtyStateChangedEvent || _raisingFileActionChangedEvent)
                throw new InvalidOperationException();
            if (filePath == null)
                throw new ArgumentNullException(nameof(filePath));
            PerformSave(overwrite ? FileMode.Create : FileMode.CreateNew, filePath, createFolder);
            UpdateSaveStatus(filePath, FilePath != filePath);
            FilePath = filePath;
        }

        public void SaveAs(string filePath, bool overwrite, IContentType newContentType)
        {
            SaveAs(filePath, overwrite, false, newContentType);
        }

        public void SaveAs(string filePath, bool overwrite, bool createFolder, IContentType newContentType)
        {
            if (newContentType == null)
                throw new ArgumentNullException(nameof(newContentType));
            SaveAs(filePath, overwrite, createFolder);
            TextBuffer.ChangeContentType(newContentType, null);
        }

        public void SaveCopy(string filePath, bool overwrite)
        {
            SaveCopy(filePath, overwrite, false);
        }

        public void SaveCopy(string filePath, bool overwrite, bool createFolder)
        {
            if (IsDisposed)
                throw new ObjectDisposedException("ITextDocument");
            if (filePath == null)
                throw new ArgumentNullException(nameof(filePath));
            PerformSave(overwrite ? FileMode.Create : FileMode.CreateNew, filePath, createFolder);
        }

        public void SetEncoderFallback(EncoderFallback fallback)
        {
            _encoding = Encoding.GetEncoding(_encoding.CodePage, fallback, _encoding.DecoderFallback);
        }

        public void UpdateDirtyState(bool isDirty, DateTime lastContentModifiedTime)
        {
            if (_raisingDirtyStateChangedEvent || _raisingFileActionChangedEvent)
                throw new InvalidOperationException();
            if (IsDisposed)
                throw new ObjectDisposedException("ITextDocument");
            _lastModifiedTimeUtc = lastContentModifiedTime;
            RaiseDirtyStateChangedEvent(isDirty);
        }

        private void PerformSave(FileMode fileMode, string filePath, bool createFolder)
        {
            if (createFolder)
            {
                var dir = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
            }
            FileUtilities.SaveSnapshot(TextBuffer.CurrentSnapshot, fileMode, _encoding, filePath);
        }

        private void ReloadBufferFromStream(Stream stream, long fileSize, EditOptions options, Encoding encoding)
        {
            using (var closingStreamReader = new EncodedStreamReader.NonStreamClosingStreamReader(stream, encoding, false))
            {
                if (TextBuffer is TextBuffer buffer)
                {
                    var newContent = TextImageLoader.Load(closingStreamReader, fileSize, FilePath,
                        out var hasConsistentLineEndings, out var longestLineLength);
                    if (!hasConsistentLineEndings)
                        buffer.Properties["InconsistentLineEndings"] = true;
                    else
                        buffer.Properties.RemoveProperty("InconsistentLineEndings");
                    buffer.Properties["LongestLineLength"] = longestLineLength;
                    buffer.ReloadContent(newContent, options, this);

                }
                else
                {
                    using (var edit = TextBuffer.CreateEdit(options, new int?(), this))
                    {
                        if (edit.Replace(new Span(0, edit.Snapshot.Length), closingStreamReader.ReadToEnd()))
                            edit.Apply();
                        else
                            edit.Cancel();
                    }
                }
            }
        }

        private void TextBufferChangedHandler(object sender, TextContentChangedEventArgs e)
        {
            if (e.EditTag == this)
                return;
            LastContentModifiedTime = DateTime.UtcNow;
            RaiseDirtyStateChangedEvent(e.AfterVersion.ReiteratedVersionNumber != _cleanReiteratedVersion);
        }

        private void UpdateSaveStatus(string filePath, bool renamed)
        {
            _lastSavedTimeUtc = new FileInfo(filePath).LastWriteTimeUtc;
            _cleanReiteratedVersion = TextBuffer.CurrentSnapshot.Version.ReiteratedVersionNumber;
            var type = FileActionTypes.ContentSavedToDisk;
            if (renamed)
                type |= FileActionTypes.DocumentRenamed;
            RaiseFileActionChangedEvent(_lastSavedTimeUtc, type, filePath);
        }

        private void RaiseFileActionChangedEvent(DateTime actionTime, FileActionTypes actionType, string filePath)
        {
            _raisingFileActionChangedEvent = true;
            try
            {
                if (((actionType & FileActionTypes.ContentLoadedFromDisk) == FileActionTypes.ContentLoadedFromDisk || 
                     (actionType & FileActionTypes.ContentSavedToDisk) == FileActionTypes.ContentSavedToDisk) && 
                    _cleanReiteratedVersion == TextBuffer.CurrentSnapshot.Version.ReiteratedVersionNumber)
                    RaiseDirtyStateChangedEvent(false);
                _textDocumentFactoryService.GuardedOperations.RaiseEvent(this, FileActionOccurred,
                    new TextDocumentFileActionEventArgs(filePath, actionTime, actionType));
            }
            finally
            {
                _raisingFileActionChangedEvent = false;
            }
        }

        private void RaiseDirtyStateChangedEvent(bool newDirtyState)
        {
            _raisingDirtyStateChangedEvent = true;
            try
            {
                if (IsDirty == newDirtyState)
                    return;
                IsDirty = newDirtyState;
                _textDocumentFactoryService.GuardedOperations.RaiseEvent(this, DirtyStateChanged);
            }
            finally
            {
                _raisingDirtyStateChangedEvent = false;
            }
        }
    }
}