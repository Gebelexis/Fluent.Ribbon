using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Fluent
{
    /// <summary>
    /// Represents size definition for group box
    /// </summary>
    [ContentProperty("Children")]
    [SuppressMessage("Microsoft.Naming", "CA1702", Justification = "We mean here 'bar row' instead of 'barrow'")]
    public class RibbonToolBarRow : DependencyObject
    {
        #region Fields

        // User defined rows
        readonly ObservableCollection<DependencyObject> children = new ObservableCollection<DependencyObject>();

        #endregion

        #region Properties
        
        /// <summary>
        /// Gets rows
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ObservableCollection<DependencyObject> Children
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
            DependencyProperty.Register("ChildrenSource", typeof(IEnumerable), typeof(RibbonToolBarRow), new UIPropertyMetadata(null, OnChildrenSourceChanged));

        static void OnChildrenSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonToolBarRow toolBarRow = (RibbonToolBarRow)d;
            ItemsSourceHelper.ItemsSourceChanged<DependencyObject>(toolBarRow, toolBarRow.Children, e, null, new Converters.RibbonToolBarControlRowChildDefinitionConverter());
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Default constructor
        /// </summary>
        public RibbonToolBarRow(){}

        #endregion
    }
}
