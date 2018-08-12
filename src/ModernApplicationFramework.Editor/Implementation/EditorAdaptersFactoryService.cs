using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Editor.Implementation
{
    [Export(typeof(IEditorAdaptersFactoryService))]
    internal class EditorAdaptersFactoryService : IEditorAdaptersFactoryService
    {
        public static IMafTextView GetMafTextViewFromTextView(ITextView textView)
        {
            if (textView == null)
                throw new ArgumentNullException(nameof(textView));
            if (!textView.Properties.TryGetProperty(typeof(IMafTextView), out IMafTextView property))
                return null;
            return property;
        }

        public static SimpleTextViewWindow GetSimpleTextViewWindowFromTextView(ITextView textView)
        {
            return GetMafTextViewFromTextView(textView) as SimpleTextViewWindow;
        }

        public IMafTextBuffer CreateTextBufferAdapter()
        {
            var adapter = new TextBufferAdapter();
            adapter.SetSite();
            return adapter;
        }

        public IMafTextBuffer CreateTextBufferAdapter(IContentType contentType)
        {
            var buffer = new TextBufferAdapter();
            buffer.SetInitialContentType(contentType);
            buffer.SetSite();
            return buffer;
        }

        public IMafTextBuffer CreateTextBufferAdapterForSecondaryBuffer(ITextBuffer secondaryBuffer)
        {
            if (secondaryBuffer == null)
                throw new ArgumentNullException(nameof(secondaryBuffer));
            var buffer = new SecondaryTextBufferAdapter {SecondaryBuffer = secondaryBuffer};
            return buffer;
        }

        public IMafTextView CreateTextViewAdapter()
        {
            var view = new TextViewAdapter();
            view.SetSite();
            return view;
        }

        public IMafTextView CreateTextViewAdapter(ITextViewRoleSet roles)
        {
            var view = new TextViewAdapter();
            view.SetInitialRoles(roles);
            view.SetSite();
            return view;
        }

        public ITextBuffer GetDataBuffer(IMafTextBuffer bufferAdapter)
        {
            var adapter = GetAdapter(bufferAdapter);
            if (!adapter.TextBufferContentInitialized)
                return null;
            return adapter._dataTextBuffer;
        }

        public ITextBuffer GetDocumentBuffer(IMafTextBuffer bufferAdapter)
        {
            var adapter = GetAdapter(bufferAdapter);
            return !adapter.TextBufferContentInitialized ? null : adapter._documentTextBuffer;
        }

        public ITextView GetTextView(IMafTextView viewAdapter)
        {
            return GetTextViewHostFromMafTextView(viewAdapter)?.TextView;
        }

        public ITextViewHost GetTextViewHost(IMafTextView viewAdapter)
        {
            return GetTextViewHostFromMafTextView(viewAdapter);
        }

        public void SetDataBuffer(IMafTextBuffer bufferAdapter, ITextBuffer dataBuffer)
        {
            GetAdapter(bufferAdapter)._dataTextBuffer = dataBuffer ?? throw new ArgumentNullException(nameof(dataBuffer));
        }

        public IMafTextBuffer GetBufferAdapter(ITextBuffer textBuffer)
        {
            if (textBuffer == null)
                throw new ArgumentNullException(nameof(textBuffer));
            return TextDocData.GetBufferAdapter(textBuffer);
        }

        public IMafTextView GetViewAdapter(ITextView textView)
        {
            return GetMafTextViewFromTextView(textView);
        }

        internal static ITextViewHost GetTextViewHostFromMafTextView(IMafTextView viewAdapter)
        {
            if (viewAdapter == null)
                throw new ArgumentNullException(nameof(viewAdapter));
            if (!(viewAdapter is IMafUserData userData))
                return null;
            object data;
            try
            {
                if (userData.GetData(MafUserDataFormat.TextViewHost, out data) != 0)
                    return null;
            }
            catch (InvalidOperationException )
            {
                return null;
            }
            return data as ITextViewHost;
        }

        private static TextDocData GetAdapter(IMafTextBuffer bufferAdapter)
        {
            if (bufferAdapter == null)
                throw new ArgumentNullException(nameof(bufferAdapter));
            var fromVsTextBuffer = TextDocData.GetDocDataFromMafTextBuffer(bufferAdapter);
            if (fromVsTextBuffer != null)
                return fromVsTextBuffer;
            throw new ArgumentException("bufferAdapter is not a VsTextDocData");
        }
    }

    public interface IMafUserData
    {
        int GetData(MafUserDataFormat format, out object pvtData);

        int SetData(MafUserDataFormat format, object vtData);
    }

    public enum MafUserDataFormat
    {
        TextViewHost,
        ContextMenuId
    }
}