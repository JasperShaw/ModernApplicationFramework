using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Caliburn.Micro;
using ModernApplicationFramework.EditorBase.Controls.SimpleTextEditor;
using ModernApplicationFramework.EditorBase.FileSupport;
using ModernApplicationFramework.EditorBase.Interfaces;
using ModernApplicationFramework.EditorBase.Interfaces.Editor;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;

namespace ModernApplicationFramework.EditorBase.EditorProvider
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
            var editorType = _editors.FirstOrDefault(x => x.EditorId == editorId)?.GetType();
            var method = typeof(IoC).GetMethod("Get");
            method = method.MakeGenericMethod(editorType);
            var editorToProve = method.Invoke(this, new object[] { null });

            if (!(editorToProve is IEditor editor))
                throw new NotSupportedException("The specified type was not from type IEditor");

            return editor;
        }

        public async Task New(IStorableEditor editor, string name)
        {
            await editor.LoadFile(StorableDocument.CreateNew(name), name);
        }

        public async Task Open(IStorableDocument document, string path)
        {
            // await document.Load(path);
        }

        public static MethodInfo GetMethod<T>(Expression<Action<T>> expr)
        {
            return ((MethodCallExpression) expr.Body)
                .Method
                .GetGenericMethodDefinition();
        }
    }
}