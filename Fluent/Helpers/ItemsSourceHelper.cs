using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace Fluent
{
    public static class ItemsSourceHelper
    {
        public static void ItemsSourceChanged<T>(DependencyObject parent, ObservableCollection<T> collection, DependencyPropertyChangedEventArgs e, DataTemplate dataTemplate = null, IValueConverter dataConverter = null,
            Func<int> getSelectedIndex = null, Action<int> setSelectedIndex = null, Action<DependencyObject, T, object> extraHandling = null)
            where T : DependencyObject
        {
            IEnumerable oldViewModelCollection = (IEnumerable)e.OldValue;
            IEnumerable newViewModelCollection = (IEnumerable)e.NewValue;

            NotifyCollectionChangedEventHandler collectionChangedHandler = (sender, args) => { ItemsSource_CollectionChanged(parent, collection, sender, args, dataTemplate, dataConverter, getSelectedIndex, setSelectedIndex, extraHandling); };

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
                ItemsSource_CollectionChanged(parent, collection, newViewModelCollection, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset), dataTemplate, dataConverter, null, null, extraHandling);
            }
        }

        private static void AddNewItem<T>(DependencyObject parent, ObservableCollection<T> collection, object item, int index, DataTemplate dataTemplate, IValueConverter dataConverter, Action<DependencyObject, T, object> extraHandling = null)
             where T : DependencyObject
        {
            if (item is T) 
            {
                collection.Insert(index, (T)item);
                return;
            }
            T dObject = null;
            if (dataConverter != null)
            {
                dObject = dataConverter.Convert(item, typeof(T), null, CultureInfo.InvariantCulture) as T;
                if (dObject is FrameworkElement)
                {
                    ((FrameworkElement)(DependencyObject)dObject).DataContext = item;
                }
            }
            else if (dataTemplate != null)
            {
                dObject = dataTemplate.LoadContent() as T;
                if (dObject is FrameworkElement)
                {
                    ((FrameworkElement)(DependencyObject)dObject).DataContext = item;
                }
            }
            else if (parent is FrameworkElement)
            {
                var defaultTemplate = ((FrameworkElement)parent).FindResource(new DataTemplateKey(item.GetType()));
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
                extraHandling(parent, dObject, item);
            if (dObject != null)
                collection.Insert(index, dObject);
        }

        private static void ItemsSource_CollectionChanged<T>(DependencyObject parent, ObservableCollection<T> collection, object sender, NotifyCollectionChangedEventArgs e, DataTemplate dataTemplate, IValueConverter dataConverter, Func<int> getSelectedIndex, Action<int> setSelectedIndex, Action<DependencyObject, T, object> extraHandling)
            where T : DependencyObject
        {
            int currentIndex = -1;
            T currentSelected = null;
            if (getSelectedIndex != null)
            {
                currentIndex = getSelectedIndex();
                if (currentIndex >= 0 && currentIndex <= collection.Count)
                    currentSelected = collection[currentIndex];
            }
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        for (var i = 0; i < e.NewItems.Count; i++)
                        {
                            AddNewItem(parent, collection, e.NewItems[i], collection.Count, dataTemplate, dataConverter, extraHandling);
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
                            AddNewItem(parent, collection, e.NewItems[i], e.NewStartingIndex + i, dataTemplate, dataConverter, extraHandling);
                        }
                        break;
                    }
                case NotifyCollectionChangedAction.Reset:
                    {
                        if (collection.Count > 0)
                        {
                            collection.Clear();
                        }
                        foreach (var item in sender as IEnumerable)
                        {
                            AddNewItem(parent, collection, item, collection.Count, dataTemplate, dataConverter, extraHandling);
                        }
                        break;
                    }
            }
            if (setSelectedIndex != null)
            {
                int selectedIndex = -1;
                if (collection.Count == 0)
                    selectedIndex = -1;
                else if (currentIndex <= collection.Count - 1 && currentIndex >= 0 && collection[currentIndex] == currentSelected)
                    selectedIndex = currentIndex;
                else if (collection.Contains(currentSelected))
                    selectedIndex = collection.IndexOf(currentSelected);
                else if (currentIndex - 1 >= 0 && currentIndex - 1 <= collection.Count)
                    selectedIndex = currentIndex - 1;
                else
                    selectedIndex = 0;
                if (selectedIndex >= 0 && selectedIndex <= collection.Count && selectedIndex != getSelectedIndex())
                    setSelectedIndex(selectedIndex);
            }
        }        

    }
}
