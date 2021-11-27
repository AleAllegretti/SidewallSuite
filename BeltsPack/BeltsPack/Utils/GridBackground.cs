using BeltsPack.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace BeltsPack.Utils
{
    public class NameToColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string input = value as string;
            switch (input)
            {
                case "Inviato": return new SolidColorBrush(Colors.LightYellow);
                case "Offerta": return new SolidColorBrush(Colors.White);
                case "Completato": return new SolidColorBrush(Colors.LightGreen);
                default: return DependencyProperty.UnsetValue;
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
