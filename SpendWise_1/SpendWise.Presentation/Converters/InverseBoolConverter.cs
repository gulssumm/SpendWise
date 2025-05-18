using System;
using System.Globalization;
using System.Windows.Data;

namespace SpendWise.Presentation.Converters
{
    /// <summary>
    /// Inverts boolean values for data binding.
    /// Useful for radio button groups where selecting one should deselect the other.
    /// </summary>
    public class InverseBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return !boolValue;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return !boolValue;
            }
            return value;
        }
    }
}