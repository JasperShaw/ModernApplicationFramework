using System;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using ModernApplicationFramework.Editor;
using ModernApplicationFramework.Editor.Implementation;
using ModernApplicationFramework.Editor.TextManager;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.TextEditor;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Extended.Demo.Modules.KeyGestureScopeTest
{
    [Export(typeof(PrimitiveTextView))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    internal class PrimitiveTextView
    {
        private IContentType _contentType;
        private IMafTextBuffer _bufferAdapter;
        private ITextView _textView;
        private IMafTextView _view;

        private const string Text =
            "This is a readonly TextView. This document uses the custom 'Text-Editor' " +
            "keyboard scope which is by default overriding the Copy Command Gesture (Ctrl+C)";


        public string ContentTypeName => "VerySimpleEditor";

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
                _bufferAdapter.InitializeContent(Text, Text.Length);

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
                    .CreateTextViewAdapter(service.CreateTextViewRoleSet("INTERACTIVE", "ZOOMABLE"));
                _view.Initialize(TextBuffer as IMafTextLines, IntPtr.Zero,
                    TextViewInitFlags.Hscroll | TextViewInitFlags.Vscroll | TextViewInitFlags.Readonly);
                if (_view is ITextEditorPropertyCategoryContainer view)
                {
                    var guid = DefGuidList.GuidEditPropCategoryViewMasterSettings;
                    if (view.GetPropertyCategory(guid, out var prop) == 0)
                    {
                        prop.SetProperty(EditPropId.ViewGeneralFontCategory,
                            DefGuidList.TextEditorCategory);
                        prop.SetProperty(EditPropId.ViewGeneralColorCategory,
                            DefGuidList.TextEditorCategory);
                    }
                }

                var options = IoC.Get<IEditorOptionsFactoryService>().GetOptions(TextView);
                options.SetOptionValue(DefaultTextViewOptions.WordWrapStyleId, WordWrapStyles.WordWrap);
                return _view;
            }
        }

        public object Content => TextViewHost.HostControl;

        public void Close()
        {
            Execute.OnUIThread(TextView.Close);
        }
    }
}
