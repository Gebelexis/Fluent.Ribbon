﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Fluent
{
    public static class ItemsSourceHelper
    {

        public static void ItemsSourceChanged<T>(FrameworkElement fElement, ObservableCollection<T> collection, DataTemplate dataTemplate, DependencyPropertyChangedEventArgs e,
            NotifyCollectionChangedEventHandler collectionChangedHandler = null, Action<FrameworkElement, T, object> extraHandling = null)
            where T : DependencyObject
        {
            IEnumerable oldViewModelCollection = (IEnumerable)e.OldValue;
            IEnumerable newViewModelCollection = (IEnumerable)e.NewValue;

            if (collectionChangedHandler == null)
            {
                collectionChangedHandler = (sender, args) => { ItemsSource_CollectionChanged(fElement, collection, dataTemplate, sender, args); };
            }

            if (oldViewModelCollection != null)
            {
                if (oldViewModelCollection is INotifyCollectionChanged)
                    ((INotifyCollectionChanged)oldViewModelCollection).CollectionChanged -= collectionChangedHandler;
                collection.Clear();
            }
            if (newViewModelCollection != null)
            {
                if (newViewModelCollection is INotifyCollectionChanged)
                    ((INotifyCollectionChanged)newViewModelCollection).CollectionChanged += collectionChangedHandler;
                collection.Clear();
                foreach (var item in newViewModelCollection)
                {
                    AddTemplatedItem(fElement, collection, dataTemplate, item, collection.Count, extraHandling);
                }
            }
        }

        private static void AddTemplatedItem<T>(FrameworkElement fElelement, ObservableCollection<T> collection, DataTemplate dataTemplate, object item, int index, Action<FrameworkElement, T, object> extraHandling = null)
             where T : DependencyObject
        {
            if (item is T) 
            {
                collection.Insert(index, (T)item);
                return;
            }
            T dObject = null;
            if (dataTemplate != null)
            {
                dObject = dataTemplate.LoadContent() as T;
                if (dObject is FrameworkElement)
                {
                    ((FrameworkElement)(DependencyObject)dObject).DataContext = item;
                }
            }
            else
            {
                var defaultTemplate = fElelement.FindResource(new DataTemplateKey(item.GetType()));
                if (defaultTemplate != null && defaultTemplate is DataTemplate)
                {
                    dObject = ((DataTemplate)defaultTemplate).LoadContent() as T;
                    if (dObject is FrameworkElement)
                    {
                        ((FrameworkElement)(DependencyObject)dObject).DataContext = item;
                    }
                }
            }
            if (extraHandling != null)
                extraHandling(fElelement, dObject, item);
            if (dObject != null)
                collection.Insert(index, dObject);
        }

        public static void ItemsSource_CollectionChanged<T>(FrameworkElement fElement, ObservableCollection<T> collection, DataTemplate dataTemplate, object sender, NotifyCollectionChangedEventArgs e, Action<FrameworkElement, T, object> extraHandling = null)
            where T : DependencyObject
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        for (var i = 0; i < e.NewItems.Count; i++)
                        {
                            AddTemplatedItem(fElement, collection, dataTemplate, e.NewItems[i], collection.Count, extraHandling);
                        }
                        break;
                    }
                case NotifyCollectionChangedAction.Move:
                    {
                        for (var i = 0; i < e.NewItems.Count; i++)
                        {
                            var dObject = collection[e.OldStartingIndex];
                            collection.RemoveAt(e.OldStartingIndex);
                            collection.Insert(e.NewStartingIndex + i, dObject);
                        }
                        break;
                    }
                case NotifyCollectionChangedAction.Remove:
                    {
                        for (var i = 0; i < e.OldItems.Count; i++)
                        {
                            collection.RemoveAt(e.OldStartingIndex);
                        }
                        break;
                    }
                case NotifyCollectionChangedAction.Replace:
                    {
                        for (var i = 0; i < e.OldItems.Count; i++)
                        {
                            var dObject = collection[e.OldStartingIndex];
                            collection.RemoveAt(e.OldStartingIndex);
                        }
                        for (var i = 0; i < e.NewItems.Count; i++)
                        {
                            AddTemplatedItem(fElement, collection, dataTemplate, e.NewItems[i], e.NewStartingIndex + i, extraHandling);
                        }
                        break;
                    }
                case NotifyCollectionChangedAction.Reset:
                    {
                        collection.Clear();
                        foreach (var item in sender as IEnumerable)
                        {
                            AddTemplatedItem(fElement, collection, dataTemplate, item, collection.Count, extraHandling);
                        }
                        break;
                    }
            }
        }

        public static int SelectorItemsSource_CollectionChanged<T>(FrameworkElement fElelement, ObservableCollection<T> collection, DataTemplate dataTemplate, object sender, NotifyCollectionChangedEventArgs e, int selectedIndex, Action<FrameworkElement, T, object> extraHandling = null)
            where T : DependencyObject
        {
            int currentIndex = selectedIndex;
            T currentSelected = collection[currentIndex];

            ItemsSource_CollectionChanged(fElelement, collection, dataTemplate, sender, e, extraHandling);

            if (collection.Count == 0)
                selectedIndex = -1;
            else if (collection.Count - 1 >= currentIndex && currentIndex >= 0 && collection[currentIndex] == currentSelected)
                selectedIndex = currentIndex;
            else if (collection.Contains(currentSelected))
                selectedIndex = collection.IndexOf(currentSelected);
            else
                selectedIndex = 0;
            return selectedIndex;
        }

    }
}
