using System;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using ModernApplicationFramework.Editor.Interop;

namespace ModernApplicationFramework.Editor.Implementation
{
    [Export(typeof(IFontAndColorDefaultsProvider))]
    [Guid(CategoryGuids.FontAndColorDefaultsProvider)]
    internal class FontAndColorDefaultsProvider : IFontAndColorDefaultsProvider
    {
        private TextEditorGroup _textEditorGroupDefaults;
        private EditorTextManagerFontAndColorCategory _editorTextManagerDefaults;
        private EditorMefFontAndColorCategory _editorMefDefaults;
        private PrinterMefFontAndColorCategory _printerMefDefaults;
        private ToolWindowGroup _toolWindowGroupDefaults;
        private PrinterTextManagerFontAndColorCategory _printerTextManagerDefaults;
        private EditorTextMarkerFontAndColorCategory _editorTextMarkerDefaults;
        private PrinterTextMarkerFontAndColorCategory _printerTextMarkerDefaults;
        private ToolTipFontAndColorCategory _toolTipDefaults;
        private CommandWindowFontAndColorCategory _commandWindowDefaults;
        private ImmediateWindowFontAndColorCategory _immediateWindowDefaults;
        private OutputWindowFontAndColorCategory _outputWindowDefaults;
        private FindResultsWindowFontAndColorCategory _findResultsWindowDefaults;
        private PrinterGroup _printerGroupDefaults;

        private TextEditorGroup TextEditorGroupDefaults => _textEditorGroupDefaults ?? (_textEditorGroupDefaults = new TextEditorGroup());

        private EditorTextManagerFontAndColorCategory EditorTextManagerDefaults => _editorTextManagerDefaults ??
                                                                                   (_editorTextManagerDefaults =
                                                                                       new EditorTextManagerFontAndColorCategory());

        private EditorMefFontAndColorCategory EditorMefDefaults => _editorMefDefaults ?? (_editorMefDefaults = new EditorMefFontAndColorCategory());

        private PrinterMefFontAndColorCategory PrinterMefDefaults => _printerMefDefaults ?? (_printerMefDefaults = new PrinterMefFontAndColorCategory());

        private ToolWindowGroup ToolWindowGroupDefaults => _toolWindowGroupDefaults ?? (_toolWindowGroupDefaults = new ToolWindowGroup());

        private PrinterTextManagerFontAndColorCategory PrinterTextManagerDefaults => _printerTextManagerDefaults ??
                                                                                     (_printerTextManagerDefaults = new PrinterTextManagerFontAndColorCategory());

        private EditorTextMarkerFontAndColorCategory EditorTextMarkerDefaults => _editorTextMarkerDefaults ??
                                                                                 (_editorTextMarkerDefaults = new EditorTextMarkerFontAndColorCategory());

        private PrinterTextMarkerFontAndColorCategory PrinterTextMarkerDefaults => _printerTextMarkerDefaults ??
                                                                                   (_printerTextMarkerDefaults = new PrinterTextMarkerFontAndColorCategory());

        private ToolTipFontAndColorCategory ToolTipDefaults => _toolTipDefaults ?? (_toolTipDefaults = new ToolTipFontAndColorCategory());

        private CommandWindowFontAndColorCategory CommandWindowDefaults => _commandWindowDefaults ?? (_commandWindowDefaults = new CommandWindowFontAndColorCategory());

        private ImmediateWindowFontAndColorCategory ImmediateWindowDefaults => _immediateWindowDefaults ??
                                                                               (_immediateWindowDefaults = new ImmediateWindowFontAndColorCategory());

        private OutputWindowFontAndColorCategory OutputWindowDefaults => _outputWindowDefaults ?? (_outputWindowDefaults = new OutputWindowFontAndColorCategory());

        private FindResultsWindowFontAndColorCategory FindResultsWindowDefaults => _findResultsWindowDefaults ??
                                                                                   (_findResultsWindowDefaults = new FindResultsWindowFontAndColorCategory());

        private PrinterGroup PrinterGroupDefaults => _printerGroupDefaults ?? (_printerGroupDefaults = new PrinterGroup());

        public object GetObject(ref Guid guidCategory)
        {
            if (guidCategory == CategoryGuids.GuidTextEditorGroup)
                return TextEditorGroupDefaults;
            if (guidCategory == CategoryGuids.GuidEditorTextManager)
                return EditorTextManagerDefaults;
            if (guidCategory == CategoryGuids.GuidEditorMef)
                return EditorMefDefaults;
            if (guidCategory == CategoryGuids.GuidPrinterMef)
                return PrinterMefDefaults;
            if (guidCategory == CategoryGuids.GuidEditorTextMarker)
                return EditorTextMarkerDefaults;
            if (guidCategory == CategoryGuids.GuidPrinterGroup)
                return PrinterGroupDefaults;
            if (guidCategory == CategoryGuids.GuidPrinterTextManager)
                return PrinterTextManagerDefaults;
            if (guidCategory == CategoryGuids.GuidPrinterMef)
                return PrinterMefDefaults;
            if (guidCategory == CategoryGuids.GuidPrinterTextMarker)
                return PrinterTextMarkerDefaults;
            if (guidCategory == CategoryGuids.GuidToolWindowGroup)
                return ToolWindowGroupDefaults;
            if (guidCategory == CategoryGuids.GuidCommandWindow)
                return CommandWindowDefaults;
            if (guidCategory == CategoryGuids.GuidImmediateWindow)
                return ImmediateWindowDefaults;
            if (guidCategory == CategoryGuids.GuidOutputWindow)
                return OutputWindowDefaults;
            if (guidCategory == CategoryGuids.GuidFindResultsWindow)
                return FindResultsWindowDefaults;
            if (guidCategory == CategoryGuids.GuidToolTip)
                return ToolTipDefaults;
            return new AllOtherWindowFontAndColorCategory("Other Category", guidCategory);
        }
    }
}