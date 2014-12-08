namespace Fluent.Converters
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// Class which enables conversion from <see cref="string"/> to <see cref="RibbonControlSizeDefinition"/>
    /// </summary>
    public class SizeDefinitionConverter : TypeConverter
    {
        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
        /// </summary>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context. </param><param name="sourceType">A <see cref="T:System.Type"/> that represents the type you want to convert from. </param>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType.IsAssignableFrom(typeof(string));
        }

        /// <summary>
        /// Converts the given object to the type of this converter, using the specified context and culture information.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Object"/> that represents the converted value.
        /// </returns>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context. </param><param name="culture">The <see cref="T:System.Globalization.CultureInfo"/> to use as the current culture. </param><param name="value">The <see cref="T:System.Object"/> to convert. </param><exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return new RibbonControlSizeDefinition(value as string);
        }
    }

    public class SizeDefinitionValueConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                throw new ArgumentException("Wrong value argument in SizeDefinitionValueConverter");
            return new RibbonControlSizeDefinition(value.ToString());
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException("Cannot convert back");
        }
        #endregion
    }    
}