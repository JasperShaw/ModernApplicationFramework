using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Caliburn.Micro;
using ModernApplicationFramework.EditorBase.Interfaces;
using ModernApplicationFramework.EditorBase.Interfaces.Layout;

namespace ModernApplicationFramework.EditorBase.EditorProvider
{

    //TODO: At some point add a FileSystemWatcher so we can edit a file in multiple editor instances without risking data gets lost
    [Export(typeof(IEditorProvider))]
    public class EditorProvider : IEditorProvider
    {
        private readonly FileDefinitionManager _fileDefinitionManager;

        [ImportingConstructor]
        public EditorProvider(FileDefinitionManager fileDefinitionManager)
        {
            _fileDefinitionManager = fileDefinitionManager;
        }

        public IEnumerable<ISupportedFileDefinition> SupportedFileDefinitions
            => _fileDefinitionManager.SupportedFileDefinitions.OrderBy(x => x.SortOrder);

        public bool Handles(string path)
        {
            var extension = Path.GetExtension(path);
            return extension != null && _fileDefinitionManager.GetDefinitionByExtension(extension) != null;
        }

        public IDocument Create(Type editorType)
        {
            var method = typeof(IoC).GetMethod("Get");
            method = method.MakeGenericMethod(editorType);
            var editorToProve = method.Invoke(this, new object[] {null});

            if (!(editorToProve is IDocument editor))
                throw new NotSupportedException("The specified type was not from type IDocument");

            return editor;
        }

        public async Task New(IStorableDocument document, string name)
        {
            await document.New(name);

        }

        public async Task Open(IStorableDocument document, string path)
        {
            await document.Load(path);
        }

        public static MethodInfo GetMethod<T>(Expression<Action<T>> expr)
        {
            return ((MethodCallExpression) expr.Body)
                .Method
                .GetGenericMethodDefinition();
        }
    }
}