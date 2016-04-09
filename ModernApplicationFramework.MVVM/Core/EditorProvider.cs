using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using ModernApplicationFramework.Caliburn;
using ModernApplicationFramework.Interfaces.Utilities;
using ModernApplicationFramework.MVVM.Interfaces;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.MVVM.Core
{
    [Export(typeof(IEditorProvider))]
    public class EditorProvider : IEditorProvider
    {
        private readonly FileDefinitionManager _fileDefinitionManager;

        [ImportingConstructor]
        public EditorProvider(FileDefinitionManager fileDefinitionManager)
        {
            _fileDefinitionManager = fileDefinitionManager;
        }

        public IEnumerable<ISupportedFileDefinition> SupportedFileDefinitions => _fileDefinitionManager.SupportedFileDefinitions;

        public bool Handles(string path)
        {
            var extension = Path.GetExtension(path);
            return extension != null && _fileDefinitionManager.GetDefinitionByExtension(extension) != null;
        }

        public IDocument Create(Type editorType)
        {
            var method = typeof(IoC).GetMethod("Get");
            method = method.MakeGenericMethod(editorType);
            var editorToProve = method.Invoke(this, new object[] { null });

            var editor = editorToProve as IDocument;
            
            if (editor == null)
                throw new NotSupportedException("The specified type was not from type IDocument");

            return editor;
        }

        public static MethodInfo GetMethod<T>(Expression<Action<T>> expr)
        {
            return ((MethodCallExpression)expr.Body)
                .Method
                .GetGenericMethodDefinition();
        }

        public async Task New(IStorableDocument document, string name)
        {
            await document.New(name);
        }

        public async Task Open(IStorableDocument document, string path)
        {
            await document.Load(path);
        }
    }
}
