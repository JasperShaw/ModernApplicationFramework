using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Caliburn.Micro;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Interfaces.Utilities;
using ModernApplicationFramework.MVVM.Interfaces;

namespace ModernApplicationFramework.MVVM.Core
{

    //TODO: At some point add a FileSystemWatcher so we can edit a file in mulpile editor instances without risking data gets lost
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

            var editor = editorToProve as IDocument;

            if (editor == null)
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