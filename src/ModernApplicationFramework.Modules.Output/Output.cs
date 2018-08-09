using System;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using ModernApplicationFramework.TextEditor;
using ModernApplicationFramework.TextEditor.Implementation;
using ModernApplicationFramework.TextEditor.Implementation.OutputClassifier;

namespace ModernApplicationFramework.Modules.Output
{
    [Export(typeof(IOutput))]
    internal class Output : IOutput, IOutputPrivate, IMafUserData
    {
        private IContentType _contentType;
        private IMafTextBuffer _bufferAdapter;
        private ITextView _textView;
        private IMafTextView _view;

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

        public event EventHandler ConsoleCleared;

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

        public void OutputString(string text)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public void Activate()
        {
            throw new NotImplementedException();
        }

        public void Hide()
        {
            throw new NotImplementedException();
        }

        private ITextViewHost TextViewHost
        {
            get
            {
                MafTextView.GetData(MafUserDataFormat.TextViewHost, out var data);
                return data as ITextViewHost;
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
                    TextViewInitFlags.Hscroll | TextViewInitFlags.Vscroll | TextViewInitFlags.ProhibitUserInput);
                if (_view is ITextEditorPropertyCategoryContainer view)
                {
                    var guid = DefGuidList.GuidEditPropCategoryViewMasterSettings;
                    if (view.GetPropertyCategory(ref guid, out var prop) == 0)
                    {
                        prop.SetProperty(VsEditPropId.ViewGeneralFontCategory,
                            DefGuidList.OutputWindowCategory);
                        prop.SetProperty(VsEditPropId.ViewGeneralColorCategory,
                            DefGuidList.OutputWindowCategory);
                    }
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
    }
}
