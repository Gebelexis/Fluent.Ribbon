using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Fluent.Converters
{
    public class GalleryGroupFilterConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                throw new ArgumentException("Wrong value argument in GalleryGroupFilterConverter");
            var viewModel = value;
            GalleryGroupFilter view = new GalleryGroupFilter();

            Binding titleBinding = new Binding("Title");
            titleBinding.Source = viewModel;
            BindingOperations.SetBinding(view, GalleryGroupFilter.TitleProperty, titleBinding);

            Binding groupsBinding = new Binding("Groups");
            groupsBinding.Source = viewModel;
            BindingOperations.SetBinding(view, GalleryGroupFilter.GroupsProperty, groupsBinding);

            return view;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Binding.DoNothing;
        }
        #endregion
    }
}
