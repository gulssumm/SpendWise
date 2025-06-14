using System;
using System.Globalization;
using System.Windows;
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

    /// <summary>
    /// Converts null to Visibility.Collapsed, non-null to Visibility.Visible
    /// </summary>
    public class NullToVisibilityConverter : IValueConverter
    {
        public static readonly NullToVisibilityConverter Instance = new NullToVisibilityConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converts decimal amounts to formatted currency strings with color coding
    /// </summary>
    public class AmountToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal amount)
            {
                return amount >= 0 ? "#27AE60" : "#E74C3C"; // Green for positive, Red for negative
            }
            return "#2C3E50"; // Default dark color
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}