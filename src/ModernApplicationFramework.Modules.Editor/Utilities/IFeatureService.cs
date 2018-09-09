using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.Utilities.Attributes;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Modules.Editor.Utilities
{
    public interface IFeatureService
    {
        bool IsEnabled(string featureName);

        IFeatureDisableToken Disable(string featureName, IFeatureController controller);

        event EventHandler<FeatureUpdatedEventArgs> StateUpdated;

        IFeatureCookie GetCookie(string featureName);
    }

    public interface IFeatureCookie
    {
        event EventHandler<FeatureChangedEventArgs> StateChanged;

        bool IsEnabled { get; }

        string FeatureName { get; }
    }

    public class FeatureChangedEventArgs : EventArgs
    {
        public string FeatureName { get; }

        public bool IsEnabled { get; }

        public FeatureChangedEventArgs(string featureName, bool isEnabled)
        {
            FeatureName = featureName;
            IsEnabled = isEnabled;
        }
    }

    public interface IFeatureDisableToken : IDisposable
    {
    }

    public interface IFeatureController
    {
    }

    public interface IFeatureServiceFactory
    {
        IFeatureService GlobalFeatureService { get; }

        IFeatureService GetOrCreate(IPropertyOwner scope);
    }

    [Export(typeof(IFeatureServiceFactory))]
    internal class FeatureServiceFactory : IFeatureServiceFactory, IPartImportsSatisfiedNotification
    {
        private bool _initializing;
        private IFeatureService _globalFeatureService;

        [ImportMany]
        internal IEnumerable<Lazy<FeatureDefinition, IFeatureDefinitionMetadata>> AllDefinitions { get; set; }

        [Import]
        internal IGuardedOperations GuardedOperations { get; set; }

        internal IDictionary<string, SortedSet<string>> RelatedDefinitions { get; set; }

        public IFeatureService GlobalFeatureService
        {
            get
            {
                if (_initializing)
                    throw new InvalidOperationException(
                        $"Do not access {nameof(GlobalFeatureService) as object} when it is being initialized");
                if (_globalFeatureService == null)
                {
                    _initializing = true;
                    _globalFeatureService = new FeatureService(null, this);
                    _initializing = false;
                }
                return _globalFeatureService;
            }
        }

        public IFeatureService GetOrCreate(IPropertyOwner scope)
        {
            if (scope == null)
                throw new ArgumentNullException(nameof(scope));
            return scope.Properties.GetOrCreateSingletonProperty(() => new FeatureService(GlobalFeatureService, this));
        }

        void IPartImportsSatisfiedNotification.OnImportsSatisfied()
        {
            RelatedDefinitions = new Dictionary<string, SortedSet<string>>();
            foreach (var allDefinition in AllDefinitions)
            {
                var sortedSet = new SortedSet<string>();
                AddBaseDefinitionNamesToSet(allDefinition.Metadata.Name, sortedSet);
                RelatedDefinitions[allDefinition.Metadata.Name] = sortedSet;
            }
        }

        private void AddBaseDefinitionNamesToSet(string name, ISet<string> set)
        {
            foreach (var lazy in AllDefinitions.Where(n => n.Metadata.Name.Equals(name, StringComparison.Ordinal)))
            {
                set.Add(lazy.Metadata.Name);
                if (lazy.Metadata.BaseDefinition != null)
                {
                    foreach (var name1 in lazy.Metadata.BaseDefinition)
                        AddBaseDefinitionNamesToSet(name1, set);
                }
            }
        }
    }

    public sealed class FeatureDefinition
    {
    }

    internal class StandardEditorFeatureDefinitions
    {
        [Export]
        [Name("Editor")]
        public FeatureDefinition EditorDefinition;
        [Export]
        [Name("Popup")]
        public FeatureDefinition PopupDefinition;
        [Export]
        [Name("InteractivePopup")]
        [BaseDefinition("Popup")]
        public FeatureDefinition InteractivePopupDefinition;
        [Export]
        [Name("Completion")]
        [BaseDefinition("InteractivePopup")]
        [BaseDefinition("Editor")]
        public FeatureDefinition CompletionDefinition;
    }

    internal class FeatureService : IFeatureService
    {
        private readonly Dictionary<string, IFeatureCookie> _cookieCache = new Dictionary<string, IFeatureCookie>();

        private IDictionary<string, FrugalList<IFeatureController>> Annulations { get; set; }

        internal IFeatureService Parent { get; }

        internal FeatureServiceFactory Factory { get; }

        public event EventHandler<FeatureUpdatedEventArgs> StateUpdated;

        internal FeatureService(IFeatureService parent, FeatureServiceFactory factory)
        {
            Parent = parent;
            Factory = factory;
            Annulations = new Dictionary<string, FrugalList<IFeatureController>>(factory.AllDefinitions.Count());
            foreach (var allDefinition in factory.AllDefinitions)
                Annulations[allDefinition.Metadata.Name] = new FrugalList<IFeatureController>();
            if (Parent == null)
                return;
            Parent.StateUpdated += OnParentServiceStateUpdated;
        }

        public bool IsEnabled(string featureName)
        {
            if (string.IsNullOrEmpty(featureName))
                throw new ArgumentNullException(nameof(featureName));
            if (!Factory.RelatedDefinitions.ContainsKey(featureName))
                throw new ArgumentOutOfRangeException(nameof(featureName), $"Feature {featureName} is not registered");
            foreach (var index in Factory.RelatedDefinitions[featureName])
            {
                if (Annulations[index].Count > 0)
                    return false;
            }
            if (Parent != null)
                return Parent.IsEnabled(featureName);
            return true;
        }

        public IFeatureDisableToken Disable(string featureName, IFeatureController controller)
        {
            if (string.IsNullOrEmpty(featureName))
                throw new ArgumentNullException(nameof(featureName));
            if (controller == null)
                throw new ArgumentNullException(nameof(controller));
            if (!Factory.RelatedDefinitions.ContainsKey(featureName))
                throw new ArgumentOutOfRangeException(nameof(featureName), $"Feature {featureName} is not registered");
            var featureDisableToken = new FeatureDisableToken(this, featureName, controller);
            var annulation = Annulations[featureName];
            if (annulation.Contains(controller))
                return featureDisableToken;
            annulation.Add(controller);
            if (annulation.Count == 1)
            {
                Factory.GuardedOperations.RaiseEvent(this, StateUpdated, new FeatureUpdatedEventArgs(featureName));
            }
            return featureDisableToken;
        }

        internal void Restore(string featureName, IFeatureController controller)
        {
            if (string.IsNullOrEmpty(featureName))
                throw new ArgumentNullException(nameof(featureName));
            if (controller == null)
                throw new ArgumentNullException(nameof(controller));
            if (!Factory.RelatedDefinitions.ContainsKey(featureName))
                throw new ArgumentOutOfRangeException(nameof(featureName), $"Feature {featureName} is not registered");
            var annulation = Annulations[featureName];
            if (!annulation.Contains(controller))
                return;
            annulation.Remove(controller);
            if (annulation.Count == 0)
            {
                Factory.GuardedOperations.RaiseEvent(this, StateUpdated, new FeatureUpdatedEventArgs(featureName));
            }
        }

        public IFeatureCookie GetCookie(string featureName)
        {
            if (string.IsNullOrEmpty(featureName))
                throw new ArgumentNullException(nameof(featureName));
            if (!Factory.RelatedDefinitions.ContainsKey(featureName))
                throw new ArgumentOutOfRangeException(nameof(featureName), $"Feature {featureName} is not registered");
            if (!_cookieCache.ContainsKey(featureName))
                _cookieCache[featureName] = new FeatureCookie(featureName, Factory.RelatedDefinitions[featureName], this);
            return _cookieCache[featureName];
        }

        private void OnParentServiceStateUpdated(object sender, FeatureUpdatedEventArgs e)
        {
            // ISSUE: reference to a compiler-generated field
            Factory.GuardedOperations.RaiseEvent(sender, StateUpdated, e);
        }
    }

    public interface IFeatureDefinitionMetadata
    {
        string Name { get; }

        [DefaultValue(null)]
        IEnumerable<string> BaseDefinition { get; }
    }

    public class FeatureUpdatedEventArgs : EventArgs
    {
        public string FeatureName { get; }

        public FeatureUpdatedEventArgs(string featureName)
        {
            FeatureName = featureName;
        }
    }

    internal class FeatureCookie : IFeatureCookie
    {
        private bool _featureIsEnabled;
        private readonly IEnumerable<string> _aliases;
        private readonly FeatureService _service;

        public string FeatureName { get; }

        public event EventHandler<FeatureChangedEventArgs> StateChanged;

        public bool IsEnabled
        {
            get => _featureIsEnabled;
            private set
            {
                if (_featureIsEnabled == value)
                    return;
                _featureIsEnabled = value;
                var stateChanged = StateChanged;
                stateChanged?.Invoke(this, new FeatureChangedEventArgs(FeatureName, value));
            }
        }

        internal FeatureCookie(string featureName, IEnumerable<string> aliases, FeatureService service)
        {
            FeatureName = featureName;
            _aliases = aliases;
            _service = service;
            IsEnabled = service.IsEnabled(FeatureName);
            _service.StateUpdated += OnStateUpdated;
        }

        private void OnStateUpdated(object sender, FeatureUpdatedEventArgs args)
        {
            if (!_aliases.Contains(args.FeatureName))
                return;
            IsEnabled = _service.IsEnabled(FeatureName);
        }
    }

    internal class FeatureDisableToken : IFeatureDisableToken
    {
        private readonly FeatureService _service;
        private readonly string _featureName;
        private readonly IFeatureController _controller;

        internal FeatureDisableToken(FeatureService service, string featureName, IFeatureController controller)
        {
            _service = service;
            _featureName = featureName;
            _controller = controller;
        }

        public void Dispose()
        {
            _service.Restore(_featureName, _controller);
        }
    }
}
