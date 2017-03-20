using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace ModernApplicationFramework.Core.ViewSources
{
    public abstract class PropertyBoundFilteringCollectionViewSource<T> : CollectionViewSource, IWeakEventListener
    {
        public static DependencyProperty BoundPropertyNameProperty;

        public string BoundPropertyName
        {
            get => GetValue(BoundPropertyNameProperty) as string;
            set => SetValue(BoundPropertyNameProperty, value);
        }

        static PropertyBoundFilteringCollectionViewSource()
        {
            BoundPropertyNameProperty = DependencyProperty.Register("BoundPropertyName", typeof(string),
                typeof(PropertyBoundFilteringCollectionViewSource<T>),
                new FrameworkPropertyMetadata(null,
                    OnBoundPropertyNameChanged));
            SourceProperty.OverrideMetadata(typeof(PropertyBoundFilteringCollectionViewSource<T>),
                new FrameworkPropertyMetadata(OnViewSourceChanged));
        }

        protected PropertyBoundFilteringCollectionViewSource()
        {
            Filter += FilterCallback;
        }

        private void FilterCallback(object sender, FilterEventArgs e)
        {
            e.Accepted = AcceptItem((T)e.Item);
        }

        protected abstract bool AcceptItem(T item);

        private static void OnBoundPropertyNameChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            ((PropertyBoundFilteringCollectionViewSource<T>)sender).OnBoundPropertyNameChanged(args);
        }

        private void OnBoundPropertyNameChanged(DependencyPropertyChangedEventArgs args)
        {
            if (Source != null)
            {
                if (args.OldValue == null)
                    AttachToCollectionItems(Source as IEnumerable, args.NewValue as string);
                if (args.NewValue == null)
                    DetachFromCollectionItems(Source as IEnumerable, args.OldValue as string);
            }
            View?.Refresh();
        }

        private static void OnViewSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            ((PropertyBoundFilteringCollectionViewSource<T>)sender).OnViewSourceChanged(args);
        }

        private void OnViewSourceChanged(DependencyPropertyChangedEventArgs args)
        {
            DetachFromCollection(args.OldValue as INotifyCollectionChanged);
            DetachFromCollectionItems(args.OldValue as IEnumerable, BoundPropertyName);
            AttachToCollection(args.NewValue as INotifyCollectionChanged);
            AttachToCollectionItems(args.NewValue as IEnumerable, BoundPropertyName);
        }

        private void AttachToCollection(INotifyCollectionChanged collection)
        {
            if (collection == null)
                return;
            // ISSUE: method pointer
            collection.CollectionChanged += OnCollectionChanged;
        }

        private void AttachToCollectionItems(IEnumerable collection, string propertyName)
        {
            if (collection == null || string.IsNullOrEmpty(propertyName))
                return;
            foreach (object obj in collection)
                AttachToItem(obj as INotifyPropertyChanged, propertyName);
        }

        private void AttachToItem(INotifyPropertyChanged item, string propertyName)
        {
            if (item == null || propertyName == null)
                return;
            PropertyChangedEventManager.AddListener(item, this, propertyName);
        }

        private void DetachFromCollection(INotifyCollectionChanged collection)
        {
            if (collection == null)
                return;
            collection.CollectionChanged -= OnCollectionChanged;
        }

        private void DetachFromCollectionItems(IEnumerable collection, string propertyName)
        {
            if (collection == null || string.IsNullOrEmpty(propertyName))
                return;
            foreach (object obj in collection)
                DetachFromItem(obj as INotifyPropertyChanged, propertyName);
        }

        private void DetachFromItem(INotifyPropertyChanged item, string propertyName)
        {
            if (item == null || propertyName == null)
                return;
            PropertyChangedEventManager.RemoveListener(item, this, propertyName);
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    AttachToCollectionItems(args.NewItems, BoundPropertyName);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    DetachFromCollectionItems(args.OldItems, BoundPropertyName);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    DetachFromCollectionItems(args.OldItems, BoundPropertyName);
                    AttachToCollectionItems(args.NewItems, BoundPropertyName);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    HandleCollectionReset(Source as IEnumerable, BoundPropertyName);
                    break;
            }
        }

        private void HandleCollectionReset(IEnumerable collection, string propertyName)
        {
            if (collection == null || string.IsNullOrEmpty(propertyName))
                return;
            foreach (object obj in collection)
            {
                DetachFromItem(obj as INotifyPropertyChanged, propertyName);
                AttachToItem(obj as INotifyPropertyChanged, propertyName);
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName != BoundPropertyName || View == null)
                return;
            View.Refresh();
        }

        bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if (!(managerType == typeof(PropertyChangedEventManager)) || !(e is PropertyChangedEventArgs))
                return false;
            OnPropertyChanged(sender, (PropertyChangedEventArgs) e);
            return true;
        }
    }
}
