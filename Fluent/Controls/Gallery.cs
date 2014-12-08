﻿#region Copyright and License Information
// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright (c) Degtyarev Daniel, Rikker Serg. 2009-2010.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license
#endregion

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;

namespace Fluent
{
    // TODO: add TemplatePart's in Gallery (!)

    /// <summary>
    /// Represents gallery control. 
    /// Usually a gallery is hosted in context menu
    /// </summary>
    [ContentProperty("Items")]
    public class Gallery : ListBox
    {
        #region Fields

        private ObservableCollection<GalleryGroupFilter> filters;

        private DropDownButton groupsMenuButton;

        #endregion

        #region Properties

        #region MinItemsInRow

        /// <summary>
        /// Min width of the Gallery 
        /// </summary>
        public int MinItemsInRow
        {
            get { return (int)GetValue(MinItemsInRowProperty); }
            set { SetValue(MinItemsInRowProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MinItemsInRow.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MinItemsInRowProperty =
            DependencyProperty.Register("MinItemsInRow", typeof(int),
            typeof(Gallery), new UIPropertyMetadata(1));

        #endregion

        #region MaxItemsInRow

        /// <summary>
        /// Max width of the Gallery 
        /// </summary>
        public int MaxItemsInRow
        {
            get { return (int)GetValue(MaxItemsInRowProperty); }
            set { SetValue(MaxItemsInRowProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MaxItemsInRow.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MaxItemsInRowProperty =
            DependencyProperty.Register("MaxItemsInRow", typeof(int),
            typeof(Gallery), new UIPropertyMetadata(Int32.MaxValue));

        #endregion

        #region GroupBy

        /// <summary>
        /// Gets or sets name of property which
        /// will use to group items in the Gallery.
        /// </summary>
        public string GroupBy
        {
            get { return (string)GetValue(GroupByProperty); }
            set { SetValue(GroupByProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for GroupBy.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty GroupByProperty =
            DependencyProperty.Register("GroupBy", typeof(string), typeof(Gallery),
            new UIPropertyMetadata(null));

        #endregion

        #region Orientation

        /// <summary>
        /// Gets or sets orientation of gallery
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Orientation.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation),
            typeof(Gallery), new UIPropertyMetadata(Orientation.Horizontal));

        #endregion

        #region ItemWidth

        /// <summary>
        /// Gets or sets item width
        /// </summary>
        public double ItemWidth
        {
            get { return (double)GetValue(ItemWidthProperty); }
            set { SetValue(ItemWidthProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ItemWidth.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ItemWidthProperty =
            DependencyProperty.Register("ItemWidth", typeof(double), typeof(Gallery), new UIPropertyMetadata(double.NaN));

        /// <summary>
        /// Gets or sets item height
        /// </summary>
        public double ItemHeight
        {
            get { return (double)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ItemHeight.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ItemHeightProperty =
            DependencyProperty.Register("ItemHeight", typeof(double), typeof(Gallery), new UIPropertyMetadata(double.NaN));

        #endregion

        #region Filters

        /// <summary>
        /// Gets collection of filters
        /// </summary>
        public ObservableCollection<GalleryGroupFilter> Filters
        {
            get
            {
                if (filters == null)
                {
                    filters = new ObservableCollection<GalleryGroupFilter>();
                    filters.CollectionChanged += OnFilterCollectionChanged;
                }
                return filters;
            }
        }

        // Handle toolbar items changes
        void OnFilterCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            HasFilter = Filters.Count > 0;
            InvalidateProperty(SelectedFilterProperty);

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    for (int i = 0; i < e.NewItems.Count; i++)
                    {
                        if (groupsMenuButton != null)
                        {
                            GalleryGroupFilter filter = (GalleryGroupFilter)e.NewItems[i];
                            MenuItem menuItem = new MenuItem();
                            menuItem.Header = filter.Title;
                            menuItem.Tag = filter;
                            if (filter == SelectedFilter) menuItem.IsChecked = true;
                            menuItem.Click += OnFilterMenuItemClick;
                            groupsMenuButton.Items.Insert(e.NewStartingIndex + i, menuItem);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (object item in e.OldItems)
                    {
                        if (groupsMenuButton != null)
                        {
                            groupsMenuButton.Items.Remove(GetFilterMenuItem(item as GalleryGroupFilter));
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (object item in e.OldItems)
                    {
                        if (groupsMenuButton != null)
                        {
                            groupsMenuButton.Items.Remove(GetFilterMenuItem(item as GalleryGroupFilter));
                        }
                    }
                    foreach (var item in e.NewItems.OfType<GalleryGroupFilter>())
                    {
                        if (groupsMenuButton != null)
                        {
                            GalleryGroupFilter filter = item;
                            MenuItem menuItem = new MenuItem();
                            menuItem.Header = filter.Title;
                            menuItem.Tag = filter;
                            if (filter == SelectedFilter) menuItem.IsChecked = true;
                            menuItem.Click += OnFilterMenuItemClick;
                            groupsMenuButton.Items.Add(menuItem);
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Gets or sets selected filter
        /// </summary>               
        public GalleryGroupFilter SelectedFilter
        {
            get { return (GalleryGroupFilter)GetValue(SelectedFilterProperty); }
            set { SetValue(SelectedFilterProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectedFilter. 
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedFilterProperty =
            DependencyProperty.Register("SelectedFilter", typeof(GalleryGroupFilter),
            typeof(Gallery), new UIPropertyMetadata(null, OnFilterChanged, CoerceSelectedFilter));

        // Coerce selected filter
        static object CoerceSelectedFilter(DependencyObject d, object basevalue)
        {
            Gallery gallery = (Gallery)d;
            if ((basevalue == null) && (gallery.Filters.Count > 0)) return gallery.Filters[0];
            return basevalue;
        }

        // Handles filter property changed
        static void OnFilterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery gallery = (Gallery)d;
            GalleryGroupFilter oldFilter = e.OldValue as GalleryGroupFilter;
            if (oldFilter != null)
            {
                System.Windows.Controls.MenuItem menuItem = gallery.GetFilterMenuItem(oldFilter);
                if (menuItem != null) menuItem.IsChecked = false;
            }
            GalleryGroupFilter filter = e.NewValue as GalleryGroupFilter;            
            if (filter != null)
            {
                gallery.SelectedFilterTitle = filter.Title;
                gallery.SelectedFilterGroups = filter.Groups;
                gallery.SelectedFilterIndex = gallery.Filters.IndexOf(filter);
                System.Windows.Controls.MenuItem menuItem = gallery.GetFilterMenuItem(filter);
                if (menuItem != null) menuItem.IsChecked = true;
            }
            else
            {
                gallery.SelectedFilterTitle = "";
                gallery.SelectedFilterGroups = null;
                gallery.SelectedFilterIndex = -1;
            }
            gallery.UpdateLayout();
        }

        /// <summary>
        /// Gets selected filter title
        /// </summary>
        public string SelectedFilterTitle
        {
            get { return (string)GetValue(SelectedFilterTitleProperty); }
            private set { SetValue(SelectedFilterTitlePropertyKey, value); }
        }

        private static readonly DependencyPropertyKey SelectedFilterTitlePropertyKey =
            DependencyProperty.RegisterReadOnly("SelectedFilterTitle", typeof(string),
            typeof(Gallery), new UIPropertyMetadata(null));

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectedFilterTitle. 
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedFilterTitleProperty = SelectedFilterTitlePropertyKey.DependencyProperty;

        /// <summary>
        /// Gets selected filter groups
        /// </summary>
        public string SelectedFilterGroups
        {
            get { return (string)GetValue(SelectedFilterGroupsProperty); }
            private set { SetValue(SelectedFilterGroupsPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey SelectedFilterGroupsPropertyKey =
            DependencyProperty.RegisterReadOnly("SelectedFilterGroups", typeof(string),
            typeof(Gallery), new UIPropertyMetadata(null));

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectedFilterGroups. 
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedFilterGroupsProperty = SelectedFilterGroupsPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets whether gallery has selected filter
        /// </summary>       
        public bool HasFilter
        {
            get { return (bool)GetValue(HasFilterProperty); }
            private set { SetValue(HasFilterPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey HasFilterPropertyKey = DependencyProperty.RegisterReadOnly("HasFilter", typeof(bool), typeof(Gallery), new UIPropertyMetadata(false));

        /// <summary>
        /// Using a DependencyProperty as the backing store for HasFilter.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HasFilterProperty = HasFilterPropertyKey.DependencyProperty;

        void OnFilterMenuItemClick(object sender, RoutedEventArgs e)
        {
            MenuItem senderItem = (MenuItem)sender;
            MenuItem item = GetFilterMenuItem(SelectedFilter);
            item.IsChecked = false;
            senderItem.IsChecked = true;
            SelectedFilter = senderItem.Tag as GalleryGroupFilter;
            groupsMenuButton.IsDropDownOpen = false;
            e.Handled = true;
        }

        MenuItem GetFilterMenuItem(GalleryGroupFilter filter)
        {
            if (filter == null) return null;
            if (groupsMenuButton == null) return null; 
            return groupsMenuButton.Items.Cast<MenuItem>().FirstOrDefault(item => (item != null) && (item.Header.ToString() == filter.Title));
            /*foreach (MenuItem item in groupsMenuButton.Items)
            {
                if ((item!=null)&&(item.Header == filter.Title)) return item;
            }
            return null;*/
        }

        #endregion

        #region Selectable

        /// <summary>
        /// Gets or sets whether gallery items can be selected
        /// </summary>
        public bool Selectable
        {
            get { return (bool)GetValue(SelectableProperty); }
            set { SetValue(SelectableProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Selectable.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectableProperty =
            DependencyProperty.Register("Selectable", typeof(bool),
            typeof(Gallery), new UIPropertyMetadata(true, OnSelectableChanged));

        private static void OnSelectableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.CoerceValue(SelectedItemProperty);
        }

        #endregion

        #region IsLastItem

        /// <summary>
        /// Gets whether gallery is last item in ItemsControl
        /// </summary>
        public bool IsLastItem
        {
            get { return (bool)GetValue(IsLastItemProperty); }
            private set { SetValue(IsLastItemPropertyKey, value); }
        }

        /// <summary>
        ///  Using a DependencyProperty as the backing store for IsLastItem.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyPropertyKey IsLastItemPropertyKey = DependencyProperty.RegisterReadOnly("IsLastItem", typeof(bool), typeof(Gallery), new UIPropertyMetadata(false));
        /// <summary>
        ///  Using a DependencyProperty as the backing store for IsLastItem.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsLastItemProperty = IsLastItemPropertyKey.DependencyProperty;

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static Gallery()
        {
            Type type = typeof(Gallery);
            DefaultStyleKeyProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(typeof(Gallery)));
            SelectedItemProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(null, CoerceSelectedItem));
            ContextMenuService.Attach(type);
            StyleProperty.OverrideMetadata(typeof(Gallery), new FrameworkPropertyMetadata(null, new CoerceValueCallback(OnCoerceStyle)));
        }

        // Coerce object style
        static object OnCoerceStyle(DependencyObject d, object basevalue)
        {
            if (basevalue == null)
            {
                basevalue = (d as FrameworkElement).TryFindResource(typeof(Gallery));
            }

            return basevalue;
        }

        // Coerce selected item
        private static object CoerceSelectedItem(DependencyObject d, object basevalue)
        {
            Gallery gallery = (Gallery)d;

            if (!gallery.Selectable)
            {
                GalleryItem galleryItem = (GalleryItem)gallery.ItemContainerGenerator.ContainerFromItem(basevalue);

                if (basevalue != null && galleryItem != null)
                {
                    galleryItem.IsSelected = false;
                }

                return null;
            }

            return basevalue;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Gallery()
        {
            ContextMenuService.Coerce(this);
            this.Loaded += this.OnLoaded;
            this.Focusable = false;
            KeyboardNavigation.SetDirectionalNavigation(this, KeyboardNavigationMode.Continue);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            ItemsControl parent = this.Parent as ItemsControl;
            if (parent != null)
            {
                if (parent.Items.IndexOf(this) == parent.Items.Count - 1)
                {
                    this.IsLastItem = true;
                }
                else
                {
                    this.IsLastItem = false;
                }
            }
        }

        #endregion

        #region Overrides

#if NET45
        private object currentItem;
#endif
        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>The element that is used to display the given item.</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
#if NET45
            var item = this.currentItem;
            this.currentItem = null;

            if (item != null)
            {
                var dataTemplate = this.ItemTemplate;
                if (dataTemplate != null)
                {
                    var dataTemplateContent = (object)dataTemplate.LoadContent();
                    if (dataTemplateContent is GalleryItem)
                    {
                        return dataTemplateContent as DependencyObject;
                    }
                }
            }
#endif
            return new GalleryItem();
        }

        /// <summary>
        /// Determines if the specified item is (or is eligible to be) its own container.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns></returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            var isItemItsOwnContainerOverride = item is GalleryItem;

#if NET45
            if (isItemItsOwnContainerOverride == false)
            {
                this.currentItem = item;
            }
#endif

            return isItemItsOwnContainerOverride;
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            if (this.groupsMenuButton != null)
            {
                this.groupsMenuButton.Items.Clear();
            }

            this.groupsMenuButton = this.GetTemplateChild("PART_DropDownButton") as DropDownButton;

            if (this.groupsMenuButton != null)
            {
                for (int i = 0; i < this.Filters.Count; i++)
                {
                    var item = new MenuItem
                        {
                            Header = this.Filters[i].Title,
                            Tag = this.Filters[i],
                            IsDefinitive = false
                        };

                    if (this.Filters[i] == this.SelectedFilter)
                    {
                        item.IsChecked = true;
                    }

                    item.Click += this.OnFilterMenuItemClick;
                    this.groupsMenuButton.Items.Add(item);
                }
            }

            base.OnApplyTemplate();
        }

        #endregion

        #region GalleryGroupFilterSource

        /// <summary>
        /// Gets or sets GalleryGroupFilterSource
        /// </summary>
        public IEnumerable GalleryGroupFilterSource
        {
            get { return (IEnumerable)GetValue(GalleryGroupFilterSourceProperty); }
            set { SetValue(GalleryGroupFilterSourceProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for GalleryGroupFilterSource. 
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty GalleryGroupFilterSourceProperty =
            DependencyProperty.Register("GalleryGroupFilterSource", typeof(IEnumerable), typeof(Gallery), new UIPropertyMetadata(null, OnGalleryGroupFilterSourceChanged));

        static void OnGalleryGroupFilterSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery gallery = (Gallery)d;
            ItemsSourceHelper.ItemsSourceChanged<GalleryGroupFilter>(gallery, gallery.Filters, e, null, new Converters.GalleryGroupFilterConverter(), () => gallery.SelectedFilterIndex, index => gallery.SelectedFilterIndex = index);
        }

        #endregion

        #region SelectedFilterIndex

        /// <summary>
        /// Gets or sets SelectedFilterIndex
        /// </summary>
        public int SelectedFilterIndex
        {
            get { return (int)GetValue(SelectedFilterIndexProperty); }
            set { SetValue(SelectedFilterIndexProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectedFilterIndex. 
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedFilterIndexProperty =
            DependencyProperty.Register("SelectedFilterIndex", typeof(int), typeof(Gallery), new UIPropertyMetadata(-1, OnSelectedFilterIndexChanged));

        static void OnSelectedFilterIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery gallery = (Gallery)d;
            var selectedIndex = (int)e.NewValue;

            if (selectedIndex >= 0
                && selectedIndex < gallery.Filters.Count)
            {
                gallery.SelectedFilter = gallery.Filters[selectedIndex];
            }
            else
            {
                gallery.SelectedFilter = null;
            }
        }

        #endregion
    }
}