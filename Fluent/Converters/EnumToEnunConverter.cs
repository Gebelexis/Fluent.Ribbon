using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Fluent.Converters
{
    public class EnumToEnumConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                throw new ArgumentException("Wrong value argument in EnumToEnumConverter");
            if (targetType == null || !targetType.IsEnum)
                throw new ArgumentException("Wrong targetType argument in EnumToEnumConverter");
            return Enum.Parse(targetType, value.ToString());
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                throw new ArgumentException("Wrong value argument in EnumToEnumConverter");
            if (targetType == null || !targetType.IsEnum)
                throw new ArgumentException("Wrong targetType argument in EnumToEnumConverter");
            return Enum.Parse(targetType, value.ToString());
        }
        #endregion
    }
}
