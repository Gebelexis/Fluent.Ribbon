using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Fluent.Converters
{
    public class RibbonToolBarControlDefinitionConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                throw new ArgumentException("Wrong value argument in GalleryGroupFilterConverter");
            var viewModel = value;
            RibbonToolBarControlDefinition view = new RibbonToolBarControlDefinition();

            Binding sizeBinding = new Binding("Size");
            sizeBinding.Source = viewModel;
            sizeBinding.Converter = new EnumToEnumConverter();
            BindingOperations.SetBinding(view, RibbonToolBarControlDefinition.SizeProperty, sizeBinding);

            Binding sizeDefinitionBinding = new Binding("SizeDefinition");
            sizeDefinitionBinding.Source = viewModel;
            sizeDefinitionBinding.Converter = new SizeDefinitionValueConverter();
            BindingOperations.SetBinding(view, RibbonToolBarControlDefinition.SizeDefinitionProperty, sizeDefinitionBinding);

            Binding targetBinding = new Binding("Target");
            targetBinding.Source = viewModel;
            BindingOperations.SetBinding(view, RibbonToolBarControlDefinition.TargetSourceProperty, targetBinding);

            Binding widthBinding = new Binding("Width");
            widthBinding.Source = viewModel;
            BindingOperations.SetBinding(view, RibbonToolBarControlDefinition.WidthProperty, widthBinding);

            return view;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Binding.DoNothing;
        }
        #endregion
    }

    public class RibbonToolBarControlRowChildDefinitionConverter : IValueConverter
    {
        private RibbonToolBarControlDefinitionConverter controlDefinitionConverter = new RibbonToolBarControlDefinitionConverter();

        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                throw new ArgumentException("Wrong value argument in RibbonToolBarControlGroupDefinitionConverter");
            var viewModel = value;

            if (!viewModel.GetType().GetProperties().Select(pi => pi.Name).Contains("Children")) //viewmodel has a ChildrenSource property?
            {
                //if not, assume this is a RibbonToolBarControlDefinition and not a group
                return controlDefinitionConverter.Convert(value, targetType, parameter, culture);
            }

            RibbonToolBarControlGroupDefinition view = new RibbonToolBarControlGroupDefinition();
            Binding childrenSourceBinding = new Binding("Children");
            childrenSourceBinding.Source = viewModel;
            BindingOperations.SetBinding(view, RibbonToolBarControlGroupDefinition.ChildrenSourceProperty, childrenSourceBinding);            

            return view;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Binding.DoNothing;
        }
        #endregion
    }

    public class RibbonToolBarRowConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                throw new ArgumentException("Wrong value argument in RibbonToolBarRowConverter");
            var viewModel = value;
            RibbonToolBarRow view = new RibbonToolBarRow();

            Binding childrenSourceBinding = new Binding("Children");
            childrenSourceBinding.Source = viewModel;
            BindingOperations.SetBinding(view, RibbonToolBarRow.ChildrenSourceProperty, childrenSourceBinding);
            
            return view;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Binding.DoNothing;
        }
        #endregion
    }

    public class RibbonToolBarLayoutDefinitionConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                throw new ArgumentException("Wrong value argument in RibbonToolBarLayoutDefinitionConverter");
            var viewModel = value;
            RibbonToolBarLayoutDefinition view = new RibbonToolBarLayoutDefinition();

            Binding sizeBinding = new Binding("Size");
            sizeBinding.Source = viewModel;
            sizeBinding.Converter = new EnumToEnumConverter();
            BindingOperations.SetBinding(view, RibbonToolBarLayoutDefinition.SizeProperty, sizeBinding);

            //Binding sizeDefinitionBinding = new Binding("SizeDefinition");
            //sizeDefinitionBinding.Source = viewModel;
            //sizeDefinitionBinding.Converter = new SizeDefinitionValueConverter();
            //BindingOperations.SetBinding(view, RibbonToolBarLayoutDefinition.SizeDefinitionProperty, sizeDefinitionBinding);

            Binding rowSourceBinding = new Binding("Rows");
            rowSourceBinding.Source = viewModel;
            BindingOperations.SetBinding(view, RibbonToolBarLayoutDefinition.RowsSourceProperty, rowSourceBinding);

            return view;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Binding.DoNothing;
        }
        #endregion
    }
}
