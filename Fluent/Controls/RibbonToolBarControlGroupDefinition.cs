using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Fluent
{
    /// <summary>
    /// Represent logical container for toolbar items
    /// </summary>
    [ContentProperty("Children")]
    public class RibbonToolBarControlGroupDefinition : DependencyObject
    {
        #region Events

        /// <summary>
        /// Occures when children has been changed
        /// </summary>
        public event NotifyCollectionChangedEventHandler ChildrenChanged;

        #endregion

        #region Fields

        // User defined rows
        readonly ObservableCollection<RibbonToolBarControlDefinition> children = new ObservableCollection<RibbonToolBarControlDefinition>();

        #endregion

        #region Children Property

        /// <summary>
        /// Gets rows
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ObservableCollection<RibbonToolBarControlDefinition> Children
        {
            get { return children; }
        }

        #endregion

        #region ChildrenSource

        /// <summary>
        /// Gets or sets ChildrenSource
        /// </summary>
        public IEnumerable ChildrenSource
        {
            get { return (IEnumerable)GetValue(ChildrenSourceProperty); }
            set { SetValue(ChildrenSourceProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ChildrenSource. 
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ChildrenSourceProperty =
            DependencyProperty.Register("ChildrenSource", typeof(IEnumerable), typeof(RibbonToolBarControlGroupDefinition), new UIPropertyMetadata(null, OnChildrenSourceChanged));

        static void OnChildrenSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonToolBarControlGroupDefinition groupDefinition = (RibbonToolBarControlGroupDefinition)d;
            ItemsSourceHelper.ItemsSourceChanged<RibbonToolBarControlDefinition>(groupDefinition, groupDefinition.Children, e, null, new Converters.RibbonToolBarControlDefinitionConverter());
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Default constructor
        /// </summary>
        public RibbonToolBarControlGroupDefinition()
        {
            children.CollectionChanged += OnChildrenCollectionChanged;
        }

        void OnChildrenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (ChildrenChanged != null) ChildrenChanged(sender, e);
        }

        #endregion
    }
}