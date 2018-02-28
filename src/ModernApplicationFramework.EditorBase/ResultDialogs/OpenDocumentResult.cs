using System;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Extended.ResultDialogs;

namespace ModernApplicationFramework.EditorBase.ResultDialogs
{
    public class OpenDocumentResult : OpenResultBase<IDocument>
    {
        private readonly IDocument _editor;
        private readonly Type _editorType;
        private readonly string _path;

#pragma warning disable 649

        [Import] private IDockingHostViewModel _shell;

#pragma warning restore 649

        public OpenDocumentResult(IDocument editor)
        {
            _editor = editor;
        }

        public OpenDocumentResult(string path)
        {
            _path = path;
        }

        public OpenDocumentResult(Type editorType)
        {
            _editorType = editorType;
        }

        public override void Execute(CoroutineExecutionContext context)
        {
            var editor = _editor ??
                         (string.IsNullOrEmpty(_path)
                             ? (IDocument) IoC.GetInstance(_editorType, null)
                             : GetEditor(_path));

            if (editor == null)
            {
                OnCompleted(null, true);
                return;
            }

            SetData?.Invoke(editor);

            OnConfigure?.Invoke(editor);

            //editor.Deactivated += (s, e) =>
            //{
            //    if (!e.WasClosed)
            //        return;

            //    OnShutDown?.Invoke(editor);
            //};

            //_shell.OpenLayoutItem(editor);

            OnCompleted(null, false);
        }

        private static IDocument GetEditor(string path)
        {
            throw new NotImplementedException();
        }
    }
}