using System;
using System.Globalization;
using System.Windows.Data;

namespace Presentation.View
{
    /// <summary>
    /// Converts boolean IsExpense property to display text
    /// </summary>
    public class BooleanToExpenseTypeConverter : IValueConverter
    {
        public static readonly BooleanToExpenseTypeConverter Instance = new BooleanToExpenseTypeConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isExpense)
            {
                return isExpense ? "💸 Expense" : "💰 Income";
            }
            return "Unknown";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Inverts boolean values for radio button binding
    /// </summary>
    public class BooleanInverterConverter : IValueConverter
    {
        public static readonly BooleanInverterConverter Instance = new BooleanInverterConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return !boolValue;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return !boolValue;
            }
            return true;
        }
    }
}