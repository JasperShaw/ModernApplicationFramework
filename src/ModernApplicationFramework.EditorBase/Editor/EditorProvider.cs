using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Threading;
using ModernApplicationFramework.EditorBase.FileSupport;
using ModernApplicationFramework.EditorBase.FileSupport.Exceptions;
using ModernApplicationFramework.EditorBase.Interfaces;
using ModernApplicationFramework.EditorBase.Interfaces.Editor;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Utilities.Interfaces;

namespace ModernApplicationFramework.EditorBase.Editor
{

    //TODO: At some point add a FileSystemWatcher so we can edit a file in multiple editor instances without risking data gets lost
    [Export(typeof(IEditorProvider))]
    public class EditorProvider : IEditorProvider
    {
        private readonly IEnumerable<IEditor> _editors;
        private readonly IFileDefinitionManager _fileDefinitionManager;

        [ImportingConstructor]
        public EditorProvider([ImportMany] IEnumerable<IEditor> editors, IFileDefinitionManager fileDefinitionManager)
        {
            _editors = editors;
            _fileDefinitionManager = fileDefinitionManager;
        }

        public IEnumerable<ISupportedFileDefinition> SupportedFileDefinitions
            => _fileDefinitionManager.SupportedFileDefinitions.OrderBy(x => x.SortOrder);

        public bool Handles(string path)
        {
            var extension = Path.GetExtension(path);
            return extension != null && _fileDefinitionManager.GetDefinitionByExtension(extension) != null;
        }

        public IEditor Get(Guid editorId)
        {
            if (editorId == Guid.Empty)
                throw new EditorException("Editor id cannot be empty");
            var editorType = _editors.FirstOrDefault(x => x.EditorId == editorId)?.GetType();
            if (editorType == null)
                throw new EditorNotFoundException("Editor with the given ID was not found");
            var editorToProve = IoC.GetInstance(editorType, null);
            if (!(editorToProve is IEditor editor))
                throw new EditorException("The specified type was not from type IEditor");
            return editor;
        }

        public void New(NewFileArguments arguments)
        {
            var editor = Get(arguments.Editor);
            if (!editor.CanHandleFile(arguments.FileDefinition))
                throw new FileNotSupportedException("The specified file is not supported by this editor");
            editor.LoadFile(StorableDocument.CreateNew(arguments.FileName), arguments.FileName);
            IoC.Get<IDockingMainWindowViewModel>().DockingHost.OpenLayoutItem(editor);
        }

        public async void Open(OpenFileArguments args)
        {
            var editor = Get(args.Editor);
            if (!editor.CanHandleFile(args.FileDefinition))
                throw new FileNotSupportedException("The specified file is not supported by this editor");

            IDocumentBase document;
            if (!args.FileDefinition.SupportedFileOperation.HasFlag(SupportedFileOperation.Create))
                document = Document.OpenExisting(args.Path);
            else
                document = StorableDocument.OpenExisting(args.Path);

            await MafTaskHelper.Run(IoC.Get<IEnvironmentVariables>().ApplicationName ,"Opening File...", async () =>
            {
                await editor.LoadFile(document, args.Name);
            });


            IoC.Get<IDockingMainWindowViewModel>().DockingHost.OpenLayoutItem(editor);
        }

        public static MethodInfo GetMethod<T>(Expression<Action<T>> expr)
        {
            return ((MethodCallExpression)expr.Body)
                .Method
                .GetGenericMethodDefinition();
        }
    }
}