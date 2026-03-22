using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBanking.Data.Converters
{
    public class DateToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return new SolidColorBrush(Color.FromRgb(0, 0, 0));

            if (value is DateTime date)
            {
                // Comparar solo fechas (sin horas)
                if (date.Date < DateTime.Now.Date)
                {
                    return new SolidColorBrush(Color.FromRgb(255, 0, 0));
                }
            }

            return new SolidColorBrush(Color.FromRgb(50, 50, 50));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new SolidColorBrush(Color.FromRgb(0, 0, 0));
        }
    }
}
