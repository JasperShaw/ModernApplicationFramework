using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Text;
using ModernApplicationFramework.Modules.Editor.Implementation;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Utilities;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Modules.Editor.Text.Implementation
{
    [Export(typeof(ITextDocumentFactoryService))]
    internal sealed class TextDocumentFactoryService : ITextDocumentFactoryService
    {
        internal static Encoding DefaultEncoding = Encoding.Default;
        private IList<Lazy<IEncodingDetector, IEncodingDetectorMetadata>> _orderedEncodingDetectors;
        internal Func<string, Stream> StreamCreator;  

        [Import]
        internal ITextBufferFactoryService BufferFactoryService { get; set; }

        [ImportMany]
        internal List<Lazy<IEncodingDetector, IEncodingDetectorMetadata>> UnorderedEncodingDetectors { get; set; }

        [Import]
        internal GuardedOperations GuardedOperations { get; set; }

        public ITextDocument CreateAndLoadTextDocument(string filePath, IContentType contentType)
        {
            return CreateAndLoadTextDocument(filePath, contentType, true, out _);
        }

        public ITextDocument CreateAndLoadTextDocument(string filePath, IContentType contentType, Encoding encoding, out bool characterSubstitutionsOccurred)
        {
            if (filePath == null)
                throw new ArgumentNullException(nameof(filePath));
            if (contentType == null)
                throw new ArgumentNullException(nameof(contentType));
            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding));
            FallbackDetector fallbackDetector = new FallbackDetector(encoding.DecoderFallback);
            var encoding1 = (Encoding)encoding.Clone();
            encoding1.DecoderFallback = fallbackDetector;
            DateTime lastModifiedTimeUtc;
            ITextBuffer textBuffer;
            using (var stream = OpenFile(filePath, out lastModifiedTimeUtc, out var fileSize))
            {
                using (var streamReader = new StreamReader(stream, encoding1, false))
                    textBuffer = BufferFactoryService.CreateTextBuffer(streamReader, contentType, fileSize, filePath);
            }
            characterSubstitutionsOccurred = fallbackDetector.FallbackOccurred;
            TextDocument textDocument = new TextDocument(textBuffer, filePath, lastModifiedTimeUtc, this, encoding, true);
            RaiseTextDocumentCreated(textDocument);
            return textDocument;
        }

        public ITextDocument CreateAndLoadTextDocument(string filePath, IContentType contentType, bool attemptUtf8Detection, out bool characterSubstitutionsOccurred)
        {
            if (filePath == null)
                throw new ArgumentNullException(nameof(filePath));
            if (contentType == null)
                throw new ArgumentNullException(nameof(contentType));
            characterSubstitutionsOccurred = false;
            Encoding encoding1;
            var textBuffer = (ITextBuffer)null;
            List<Lazy<IEncodingDetector, IEncodingDetectorMetadata>> encodingDetectorExtensions = ExtensionSelector.SelectMatchingExtensions(OrderedEncodingDetectors, contentType);
            DateTime lastModifiedTimeUtc;
            using (var stream = OpenFile(filePath, out lastModifiedTimeUtc, out var fileSize))
            {
                encoding1 = EncodedStreamReader.DetectEncoding(stream, encodingDetectorExtensions, GuardedOperations);
                if (encoding1 == null & attemptUtf8Detection)
                {
                    try
                    {
                        ExtendedCharacterDetector characterDetector = new ExtendedCharacterDetector();
                        using (var streamReader = (StreamReader)new EncodedStreamReader.NonStreamClosingStreamReader(stream, characterDetector, false))
                        {
                            textBuffer = BufferFactoryService.CreateTextBuffer(streamReader, contentType, fileSize, filePath);
                            characterSubstitutionsOccurred = false;
                        }
                        encoding1 = !characterDetector.DecodedExtendedCharacters ? DefaultEncoding : new UTF8Encoding(false);
                    }
                    catch (DecoderFallbackException)
                    {
                        textBuffer = null;
                        stream.Position = 0L;
                    }
                }
                if (encoding1 == null)
                    encoding1 = DefaultEncoding;
                if (textBuffer == null)
                {
                    FallbackDetector fallbackDetector = new FallbackDetector(encoding1.DecoderFallback);
                    var encoding2 = (Encoding)encoding1.Clone();
                    encoding2.DecoderFallback = fallbackDetector;
                    using (var streamReader = (StreamReader)new EncodedStreamReader.NonStreamClosingStreamReader(stream, encoding2, false))
                        textBuffer = BufferFactoryService.CreateTextBuffer(streamReader, contentType, fileSize, filePath);
                    characterSubstitutionsOccurred = fallbackDetector.FallbackOccurred;
                }
            }
            TextDocument textDocument = new TextDocument(textBuffer, filePath, lastModifiedTimeUtc, this, encoding1, false, attemptUtf8Detection);
            RaiseTextDocumentCreated(textDocument);
            return textDocument;
        }

        public ITextDocument CreateTextDocument(ITextBuffer textBuffer, string filePath)
        {
            if (textBuffer == null)
                throw new ArgumentNullException(nameof(textBuffer));
            if (filePath == null)
                throw new ArgumentNullException(nameof(filePath));
            TextDocument textDocument = new TextDocument(textBuffer, filePath, DateTime.UtcNow, this, Encoding.UTF8, false, true);
            RaiseTextDocumentCreated(textDocument);
            return textDocument;
        }

        public bool TryGetTextDocument(ITextBuffer textBuffer, out ITextDocument textDocument)
        {
            if (textBuffer == null)
                throw new ArgumentNullException(nameof(textBuffer));
            textDocument = null;
            if (!textBuffer.Properties.TryGetProperty(typeof(ITextDocument), out TextDocument property) || property == null || property.IsDisposed)
                return false;
            textDocument = property;
            return true;
        }

        public event EventHandler<TextDocumentEventArgs> TextDocumentCreated;

        public event EventHandler<TextDocumentEventArgs> TextDocumentDisposed;

        private void RaiseTextDocumentCreated(ITextDocument textDocument)
        {
            var textDocumentCreated = TextDocumentCreated;
            textDocumentCreated?.Invoke(this, new TextDocumentEventArgs(textDocument));
        }

        internal void RaiseTextDocumentDisposed(ITextDocument textDocument)
        {
            var documentDisposed = TextDocumentDisposed;
            documentDisposed?.Invoke(this, new TextDocumentEventArgs(textDocument));
        }

        internal IEnumerable<Lazy<IEncodingDetector, IEncodingDetectorMetadata>> OrderedEncodingDetectors
        {
            get => _orderedEncodingDetectors ?? (_orderedEncodingDetectors = UnorderedEncodingDetectors == null
                       ? new List<Lazy<IEncodingDetector, IEncodingDetectorMetadata>>()
                       : Orderer.Order(UnorderedEncodingDetectors));
            set => _orderedEncodingDetectors = new List<Lazy<IEncodingDetector, IEncodingDetectorMetadata>>(value);
        }

        private Stream OpenFile(string filePath, out DateTime lastModifiedTimeUtc, out long fileSize)
        {
            if (StreamCreator == null)
                return OpenFileGuts(filePath, out lastModifiedTimeUtc, out fileSize);
            lastModifiedTimeUtc = DateTime.UtcNow;
            fileSize = -1L;
            return StreamCreator(filePath);
        }

        internal static Stream OpenFileGuts(string filePath, out DateTime lastModifiedTimeUtc, out long fileSize)
        {
            var fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Delete);
            var fileInfo = new FileInfo(filePath);
            lastModifiedTimeUtc = fileInfo.LastWriteTimeUtc;
            fileSize = fileInfo.Length;
            if (fileSize <= int.MaxValue)
                return fileStream;
            throw new InvalidOperationException();
        }

        internal void Initialize(ITextBufferFactoryService bufferFactoryService)
        {
            Initialize(bufferFactoryService, null);
        }

        internal void Initialize(ITextBufferFactoryService bufferFactoryService, List<Lazy<IEncodingDetector, IEncodingDetectorMetadata>> detectors)
        {
            BufferFactoryService = bufferFactoryService;
            UnorderedEncodingDetectors = detectors ?? new List<Lazy<IEncodingDetector, IEncodingDetectorMetadata>>();
        }
    }
}