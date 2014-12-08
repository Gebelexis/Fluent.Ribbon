using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Fluent
{
    /// <summary>
    /// Represents size definition for group box
    /// </summary>
    [ContentProperty("Rows")]
    public class RibbonToolBarLayoutDefinition : DependencyObject
    {
        #region Fields

        // User defined rows
        ObservableCollection<RibbonToolBarRow> rows = new ObservableCollection<RibbonToolBarRow>();

        #endregion

        #region Properties

        #region Size

        /// <summary>
        /// Gets or sets Size for the element.
        /// </summary>
        public RibbonControlSize Size
        {
            get { return (RibbonControlSize)GetValue(SizeProperty); }
            set { SetValue(SizeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Size.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SizeProperty = RibbonProperties.SizeProperty.AddOwner(typeof(RibbonToolBarLayoutDefinition));

        #endregion

        #region SizeDefinition

        /// <summary>
        /// Gets or sets SizeDefinition for element.
        /// </summary>
        public RibbonControlSizeDefinition SizeDefinition
        {
            get { return (RibbonControlSizeDefinition)GetValue(SizeDefinitionProperty); }
            set { SetValue(SizeDefinitionProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SizeDefinition.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SizeDefinitionProperty = RibbonProperties.SizeDefinitionProperty.AddOwner(typeof(RibbonToolBarLayoutDefinition));

        #endregion

        #region Row Count

        /// <summary>
        /// Gets or sets count of rows in the ribbon toolbar
        /// </summary>
        public int RowCount
        {
            get { return (int)GetValue(RowCountProperty); }
            set { SetValue(RowCountProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for RowCount.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RowCountProperty =
            DependencyProperty.Register("RowCount", typeof(int), typeof(RibbonToolBar), new UIPropertyMetadata(3));


        #endregion


        /// <summary>
        /// Gets rows
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ObservableCollection<RibbonToolBarRow> Rows
        {
            get { return rows; }
        }

        #region RowsSource

        /// <summary>
        /// Gets or sets RowsSource
        /// </summary>
        public IEnumerable RowsSource
        {
            get { return (IEnumerable)GetValue(RowsSourceProperty); }
            set { SetValue(RowsSourceProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for RowsSource. 
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RowsSourceProperty =
            DependencyProperty.Register("RowsSource", typeof(IEnumerable), typeof(RibbonToolBarLayoutDefinition), new UIPropertyMetadata(null, OnRowsSourceChanged));

        static void OnRowsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonToolBarLayoutDefinition layoutDefinition = (RibbonToolBarLayoutDefinition)d;
            ItemsSourceHelper.ItemsSourceChanged<RibbonToolBarRow>(layoutDefinition, layoutDefinition.Rows, e, null, new Converters.RibbonToolBarRowConverter());
        }

        #endregion

        #endregion
    }
}
