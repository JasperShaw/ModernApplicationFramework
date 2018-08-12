using System.Collections.Generic;
using System.Linq;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Logic.Classification;

namespace ModernApplicationFramework.Modules.Editor.Classification
{
    internal class ClassificationTypeImpl : IClassificationType
    {
        private FrugalList<IClassificationType> _baseTypes;

        public IEnumerable<IClassificationType> BaseTypes
        {
            get
            {
                if (_baseTypes == null)
                    return Enumerable.Empty<IClassificationType>();
                return _baseTypes.AsReadOnly();
            }
        }

        public string Classification { get; }

        internal ClassificationTypeImpl(string name)
        {
            Classification = name;
        }

        public bool IsOfType(string type)
        {
            if (Classification == type)
                return true;
            if (_baseTypes != null)
                foreach (var baseType in _baseTypes)
                    if (baseType.IsOfType(type))
                        return true;
            return false;
        }

        public override string ToString()
        {
            return Classification;
        }

        internal void AddBaseType(IClassificationType baseType)
        {
            if (_baseTypes == null)
                _baseTypes = new FrugalList<IClassificationType>();
            _baseTypes.Add(baseType);
        }
    }
}