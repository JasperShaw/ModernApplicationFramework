using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using ModernApplicationFramework.Text.Logic.Classification;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Modules.Editor.Classification
{
    [Export(typeof(IClassificationTypeRegistryService))]
    internal sealed class ClassificationTypeRegistryService : IClassificationTypeRegistryService
    {
        [Export]
        [Name("(TRANSIENT)")]
        public ClassificationTypeDefinition transientClassificationType;
        [Export]
        [Name("text")]
        public ClassificationTypeDefinition TextClassificationType;
        private Dictionary<string, ClassificationTypeImpl> _classificationTypes;
        private Dictionary<string, ClassificationTypeImpl> _transientClassificationTypes;

        [ImportMany]
        internal List<Lazy<ClassificationTypeDefinition, IClassificationTypeDefinitionMetadata>> ClassificationTypeDefinitions { get; set; }

        public IClassificationType GetClassificationType(string type)
        {
            ClassificationTypes.TryGetValue(type, out var classificationTypeImpl);
            return classificationTypeImpl;
        }

        public IClassificationType CreateClassificationType(string type, IEnumerable<IClassificationType> baseTypes)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (baseTypes == null)
                throw new ArgumentNullException(nameof(baseTypes));
            if (ClassificationTypes.ContainsKey(type))
                throw new InvalidOperationException();
            var classificationTypeImpl = new ClassificationTypeImpl(type);
            foreach (var baseType in baseTypes)
                classificationTypeImpl.AddBaseType(baseType);
            ClassificationTypes.Add(type, classificationTypeImpl);
            return classificationTypeImpl;
        }

        public IClassificationType CreateTransientClassificationType(IEnumerable<IClassificationType> baseTypes)
        {
            if (baseTypes == null)
                throw new ArgumentNullException(nameof(baseTypes));
            if (!baseTypes.GetEnumerator().MoveNext())
                throw new InvalidOperationException();
            return BuildTransientClassificationType(baseTypes);
        }

        public IClassificationType CreateTransientClassificationType(params IClassificationType[] baseTypes)
        {
            if (baseTypes == null)
                throw new ArgumentNullException(nameof(baseTypes));
            if (baseTypes.Length == 0)
                throw new InvalidOperationException();
            return BuildTransientClassificationType(baseTypes);
        }

        private IClassificationType TransientClassificationType => ClassificationTypes["(TRANSIENT)"];

        private Dictionary<string, ClassificationTypeImpl> ClassificationTypes
        {
            get
            {
                if (_classificationTypes == null)
                {
                    _classificationTypes = new Dictionary<string, ClassificationTypeImpl>(StringComparer.InvariantCultureIgnoreCase);
                    BuildClassificationTypes(_classificationTypes);
                }
                return _classificationTypes;
            }
        }

        private void BuildClassificationTypes(Dictionary<string, ClassificationTypeImpl> classificationTypes)
        {
            foreach (var classificationTypeDefinition in ClassificationTypeDefinitions)
            {
                var name = classificationTypeDefinition.Metadata.Name;
                if (!classificationTypes.TryGetValue(name, out var classificationTypeImpl1))
                {
                    classificationTypeImpl1 = new ClassificationTypeImpl(name);
                    classificationTypes.Add(name, classificationTypeImpl1);
                }
                var baseDefinition = classificationTypeDefinition.Metadata.BaseDefinition;
                if (baseDefinition != null)
                {
                    foreach (var str in baseDefinition)
                    {
                        if (!classificationTypes.TryGetValue(str, out var classificationTypeImpl2))
                        {
                            classificationTypeImpl2 = new ClassificationTypeImpl(str);
                            classificationTypes.Add(str, classificationTypeImpl2);
                        }
                        classificationTypeImpl1.AddBaseType(classificationTypeImpl2);
                    }
                }
            }
        }

        private IClassificationType BuildTransientClassificationType(IEnumerable<IClassificationType> baseTypes)
        {
            if (_transientClassificationTypes == null)
                _transientClassificationTypes = new Dictionary<string, ClassificationTypeImpl>(StringComparer.InvariantCultureIgnoreCase);
            var classificationTypeList = new List<IClassificationType>(baseTypes);
            classificationTypeList.Sort((a, b) => string.CompareOrdinal(a.Classification, b.Classification));
            var stringBuilder = new StringBuilder();
            foreach (var classificationType in classificationTypeList)
            {
                stringBuilder.Append(classificationType.Classification);
                stringBuilder.Append(" - ");
            }
            stringBuilder.Append(TransientClassificationType.Classification);
            if (!_transientClassificationTypes.TryGetValue(stringBuilder.ToString(), out var classificationTypeImpl))
            {
                classificationTypeImpl = new ClassificationTypeImpl(stringBuilder.ToString());
                foreach (var baseType in classificationTypeList)
                    classificationTypeImpl.AddBaseType(baseType);
                classificationTypeImpl.AddBaseType(TransientClassificationType);
                _transientClassificationTypes[classificationTypeImpl.Classification] = classificationTypeImpl;
            }
            return classificationTypeImpl;
        }
    }
}