﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using ModernApplicationFramework.Caliburn.Collections;
using ModernApplicationFramework.Caliburn.Extensions;
using ModernApplicationFramework.Caliburn.Interfaces;

namespace ModernApplicationFramework.Caliburn.Conductor
{
    public partial class Conductor<T>
    {
        /// <summary>
        ///     An implementation of <see cref="IConductor" /> that holds on many items.
        /// </summary>
        public partial class Collection
        {
            /// <summary>
            ///     An implementation of <see cref="IConductor" /> that holds on many items but only activates one at a time.
            /// </summary>
            public class OneActive : ConductorBaseWithActiveItem<T>
            {
                private readonly BindableCollection<T> _items = new BindableCollection<T>();

                /// <summary>
                ///     Initializes a new instance of the <see cref="Conductor&lt;T&gt;.Collection.OneActive" /> class.
                /// </summary>
                public OneActive()
                {
                    _items.CollectionChanged += (s, e) =>
                    {
                        switch (e.Action)
                        {
                            case NotifyCollectionChangedAction.Add:
                                e.NewItems.OfType<IChild>().Apply(x => x.Parent = this);
                                break;
                            case NotifyCollectionChangedAction.Remove:
                                e.OldItems.OfType<IChild>().Apply(x => x.Parent = null);
                                break;
                            case NotifyCollectionChangedAction.Replace:
                                e.NewItems.OfType<IChild>().Apply(x => x.Parent = this);
                                e.OldItems.OfType<IChild>().Apply(x => x.Parent = null);
                                break;
                            case NotifyCollectionChangedAction.Reset:
                                _items.OfType<IChild>().Apply(x => x.Parent = this);
                                break;
                        }
                    };
                }

                /// <summary>
                ///     Gets the items that are currently being conducted.
                /// </summary>
                public IObservableCollection<T> Items => _items;

                /// <summary>
                ///     Activates the specified item.
                /// </summary>
                /// <param name="item">The item to activate.</param>
                public override void ActivateItem(T item)
                {
                    if (item != null && item.Equals(ActiveItem))
                    {
                        if (IsActive)
                        {
                            ScreenExtensions.TryActivate(item);
                            OnActivationProcessed(item, true);
                        }

                        return;
                    }

                    ChangeActiveItem(item, false);
                }

                /// <summary>
                ///     Called to check whether or not this instance can close.
                /// </summary>
                /// <param name="callback">The implementor calls this action with the result of the close check.</param>
                public override void CanClose(Action<bool> callback)
                {
                    CloseStrategy.Execute(_items.ToList(), (canClose, closable) =>
                    {
                        var enumerable = closable as T[] ?? closable.ToArray();
                        if (!canClose && enumerable.Any())
                        {
                            if (enumerable.Contains(ActiveItem))
                            {
                                var list = _items.ToList();
                                var next = ActiveItem;
                                do
                                {
                                    var previous = next;
                                    next = DetermineNextItemToActivate(list, list.IndexOf(previous));
                                    list.Remove(previous);
                                } while (enumerable.Contains(next));

                                var previousActive = ActiveItem;
                                ChangeActiveItem(next, true);
                                _items.Remove(previousActive);

                                var stillToClose = enumerable.ToList();
                                stillToClose.Remove(previousActive);
                            }

                            enumerable.OfType<IDeactivate>().Apply(x => x.Deactivate(true));
                            _items.RemoveRange(enumerable);
                        }

                        callback(canClose);
                    });
                }

                /// <summary>
                ///     Deactivates the specified item.
                /// </summary>
                /// <param name="item">The item to close.</param>
                /// <param name="close">Indicates whether or not to close the item after deactivating it.</param>
                public override void DeactivateItem(T item, bool close)
                {
                    if (item == null)
                    {
                        return;
                    }

                    if (!close)
                    {
                        ScreenExtensions.TryDeactivate(item, false);
                    }
                    else
                    {
                        CloseStrategy.Execute(new[] {item}, (canClose, closable) =>
                        {
                            if (canClose)
                            {
                                CloseItemCore(item);
                            }
                        });
                    }
                }

                /// <summary>
                ///     Gets the children.
                /// </summary>
                /// <returns>The collection of children.</returns>
                public override IEnumerable<T> GetChildren()
                {
                    return _items;
                }

                /// <summary>
                ///     Determines the next item to activate based on the last active index.
                /// </summary>
                /// <param name="list">The list of possible active items.</param>
                /// <param name="lastIndex">The index of the last active item.</param>
                /// <returns>The next item to activate.</returns>
                /// <remarks>Called after an active item is closed.</remarks>
                protected virtual T DetermineNextItemToActivate(IList<T> list, int lastIndex)
                {
                    var toRemoveAt = lastIndex - 1;

                    if (toRemoveAt == -1 && list.Count > 1)
                    {
                        return list[1];
                    }

                    if (toRemoveAt > -1 && toRemoveAt < list.Count - 1)
                    {
                        return list[toRemoveAt];
                    }

                    return default(T);
                }

                /// <summary>
                ///     Ensures that an item is ready to be activated.
                /// </summary>
                /// <param name="newItem">The item that is about to be activated.</param>
                /// <returns>The item to be activated.</returns>
                protected override T EnsureItem(T newItem)
                {
                    if (newItem == null)
                    {
                        newItem = DetermineNextItemToActivate(_items,
                            ActiveItem != null ? _items.IndexOf(ActiveItem) : 0);
                    }
                    else
                    {
                        var index = _items.IndexOf(newItem);

                        if (index == -1)
                            _items.Add(newItem);
                        else
                            newItem = _items[index];
                    }

                    return base.EnsureItem(newItem);
                }

                /// <summary>
                ///     Called when activating.
                /// </summary>
                protected override void OnActivate()
                {
                    ScreenExtensions.TryActivate(ActiveItem);
                }

                /// <summary>
                ///     Called when deactivating.
                /// </summary>
                /// <param name="close">Inidicates whether this instance will be closed.</param>
                protected override void OnDeactivate(bool close)
                {
                    if (close)
                    {
                        _items.OfType<IDeactivate>().Apply(x => x.Deactivate(true));
                        _items.Clear();
                    }
                    else
                    {
                        ScreenExtensions.TryDeactivate(ActiveItem, false);
                    }
                }

                private void CloseItemCore(T item)
                {
                    if (item.Equals(ActiveItem))
                    {
                        var index = _items.IndexOf(item);
                        var next = DetermineNextItemToActivate(_items, index);

                        ChangeActiveItem(next, true);
                    }
                    else
                    {
                        ScreenExtensions.TryDeactivate(item, true);
                    }

                    _items.Remove(item);
                }
            }
        }
    }
}