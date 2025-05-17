using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace SpendWise.Presentation.Converters
{
    /// <summary>
    /// Converts boolean expense indicators or balance values to appropriate colors.
    /// Red for expenses or negative balances, green for income or positive balances.
    /// </summary>
    public class ExpenseColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // For balance display - check if we're processing a balance value
            if (parameter != null && parameter.ToString() == "Balance")
            {
                if (value is decimal balance)
                {
                    return balance >= 0 ? Brushes.Green : Brushes.Red;
                }
                return Brushes.Black;
            }

            // For transaction amounts - expense (true) is red, income (false) is green
            if (value is bool isExpense)
            {
                return isExpense ? Brushes.Red : Brushes.Green;
            }

            // Default color
            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // This converter is for one-way binding only
            throw new NotImplementedException();
        }
    }
}