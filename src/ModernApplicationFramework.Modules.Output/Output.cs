using System;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using ModernApplicationFramework.Editor;
using ModernApplicationFramework.Editor.Implementation;
using ModernApplicationFramework.Editor.OutputClassifier;
using ModernApplicationFramework.Editor.TextManager;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.TextEditor;
using ModernApplicationFramework.Utilities;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Modules.Output
{
    [Export(typeof(IOutput))]
    internal class Output : IOutput, IOutputPrivate, IMafUserData
    {
        private IContentType _contentType;
        private IMafTextBuffer _bufferAdapter;
        private ITextView _textView;
        private IMafTextView _view;

        private IReadOnlyRegion _readOnlyRegionBegin;
        private IReadOnlyRegion _readOnlyRegionBody;

        public event EventHandler ConsoleCleared;

        public string ContentTypeName => "Output";

        private IContentType ContentType
        {
            get
            {
                if (_contentType == null)
                {
                    var factory = IoC.Get<IContentTypeRegistryService>();
                    _contentType = factory.GetContentType(ContentTypeName) ??
                                   factory.AddContentType(ContentTypeName, new[] {"text"});
                }

                return _contentType;
            }
        }

        private IMafTextBuffer TextBuffer
        {
            get
            {
                if (_bufferAdapter != null)
                    return _bufferAdapter;
                _bufferAdapter = IoC.Get<IEditorAdaptersFactoryService>().CreateTextBufferAdapter(ContentType);
                _bufferAdapter.InitializeContent(string.Empty, 0);

                return _bufferAdapter;
            }
        }

        private ITextViewHost TextViewHost
        {
            get
            {
                MafTextView.GetData(MafUserDataFormat.TextViewHost, out var data);
                return data as ITextViewHost;
            }
        }

        private ITextView TextView
        {
            get
            {
                if (_textView != null)
                    return _textView;
                var textView = _textView = IoC.Get<IEditorAdaptersFactoryService>().GetTextView(MafTextView);
                textView.VisualElement.AllowDrop = false;
                return textView;
            }
        }

        private IMafTextView MafTextView
        {
            get
            {
                if (_view != null)
                    return _view;
                var service = IoC.Get<ITextEditorFactoryService>();
                _view = IoC.Get<IEditorAdaptersFactoryService>()
                    .CreateTextViewAdapter(service.CreateTextViewRoleSet("INTERACTIVE"));
                _view.Initialize(TextBuffer as IMafTextLines, IntPtr.Zero,
                    TextViewInitFlags.Hscroll | TextViewInitFlags.Vscroll | TextViewInitFlags.Readonly);
                if (_view is ITextEditorPropertyCategoryContainer view)
                {
                    var guid = DefGuidList.GuidEditPropCategoryViewMasterSettings;
                    if (view.GetPropertyCategory(ref guid, out var prop) == 0)
                    {
                        prop.SetProperty(EditPropId.ViewGeneralFontCategory,
                            DefGuidList.OutputWindowCategory);
                        prop.SetProperty(EditPropId.ViewGeneralColorCategory,
                            DefGuidList.OutputWindowCategory);
                    }
                }

                if (_view is IMafUserData userData)
                {
                    userData.SetData(MafUserDataFormat.ContextMenuId,
                        new Guid("{18E35741-10B1-47C0-87F0-83058900B907}"));
                }
                TextView.TextBuffer.Properties.AddProperty(typeof(IOutput), this);
                var options = IoC.Get<IEditorOptionsFactoryService>().GetOptions(TextView);
                options.SetOptionValue(DefaultTextViewOptions.WordWrapStyleId, WordWrapStyles.WordWrap);
                new OutputWindowStyleManager(this);
                return _view;
            }
        }

        public object Content => TextViewHost.HostControl;

        ~Output()
        {
            Dispose(false);
        }

        public void OutputString(string text)
        {
            if (!text.EndsWith(Environment.NewLine))
                text += Environment.NewLine;
                SetReadOnlyRegionType(ReadOnlyRegionType.None);
            var buffer = TextView.TextBuffer;
            buffer.Insert(buffer.CurrentSnapshot.Length, text);
            TextView.Caret.EnsureVisible();
            SetReadOnlyRegionType(ReadOnlyRegionType.All);
        }

        public void Clear()
        {
            SetReadOnlyRegionType(ReadOnlyRegionType.None);
            var buffer = TextView.TextBuffer;
            buffer.Delete(new Span(0, buffer.CurrentSnapshot.Length));
            ConsoleCleared.RaiseEvent(this, null);
        }

        public void Activate()
        {
            IoC.Get<IDockingHostViewModel>().ShowTool<IOutputPane>();
        }

        public void Hide()
        {
            IoC.Get<IDockingHostViewModel>().HideTool<IOutputPane>(false);
        }

        public void Dispose()
        {
            try
            {
                Dispose(true);
            }
            finally
            {
                GC.SuppressFinalize(this);
            }
            
        }

        public int GetData(MafUserDataFormat format, out object pvtData)
        {
            pvtData = null;
            if (format == MafUserDataFormat.TextViewHost)
                pvtData = TextViewHost;
            return 0;
        }

        public int SetData(MafUserDataFormat format, object vtData)
        {
            throw new NotSupportedException();
        }

        protected void Dispose(bool disposing)
        {
            if (!disposing)
                return;
            if (_bufferAdapter != null)
            {
                if (_bufferAdapter is IPersistDocData persistDoc)
                {
                    try
                    {
                        persistDoc.Close();
                    }
                    catch
                    {
                    }
                }

                _bufferAdapter = null;
            }
        }

        private void SetReadOnlyRegionType(ReadOnlyRegionType value)
        {
            var buffer = TextView.TextBuffer;
            var currentSnapshot = buffer.CurrentSnapshot;
            using (var readOnlyRegionEdit = buffer.CreateReadOnlyRegionEdit())
            {
                if (_readOnlyRegionBegin != null)
                    readOnlyRegionEdit.RemoveReadOnlyRegion(_readOnlyRegionBegin);
                if (_readOnlyRegionBody != null)
                    readOnlyRegionEdit.RemoveReadOnlyRegion(_readOnlyRegionBody);
                switch (value)
                {
                    case ReadOnlyRegionType.BeginAndBody:
                        if (currentSnapshot.Length > 0)
                        {
                            _readOnlyRegionBegin = readOnlyRegionEdit.CreateReadOnlyRegion(new Span(0, 0),
                                SpanTrackingMode.EdgeExclusive, EdgeInsertionMode.Deny);
                            _readOnlyRegionBody = readOnlyRegionEdit.CreateReadOnlyRegion(new Span(0, currentSnapshot.Length));
                        }
                        break;
                    case ReadOnlyRegionType.All:
                        _readOnlyRegionBody = readOnlyRegionEdit.CreateReadOnlyRegion(new Span(0, currentSnapshot.Length), SpanTrackingMode.EdgeExclusive, EdgeInsertionMode.Deny);
                        break;
                }

                readOnlyRegionEdit.Apply();
            }
        }

        private enum ReadOnlyRegionType
        {
            None,
            BeginAndBody,
            All,
        }
    }
}
