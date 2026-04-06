using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Graphics;

namespace HomeBanking.Data.Converters
{
    public class DateToColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is DateTime date)
            {
                var today = DateTime.Now.Date;

                if (date.Date < today)
                {
                    return Colors.Red;
                }
                else if (date.Date <= today.AddDays(3))
                {
                    return Colors.DarkOrange;
                }
                else
                {
                    return Color.FromArgb("FF2E7D32");
                }
            }

            return Colors.Gray;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return new SolidColorBrush(Color.FromRgb(0, 0, 0));
        }
    }
}
