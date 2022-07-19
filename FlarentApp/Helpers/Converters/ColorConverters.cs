using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace FlarentApp.Helpers.Converters
{
    public class ColorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return null;
            }
            else
            {
                Brush brush = StringToColor.GetSolidColorBrush(value.ToString());
                return brush;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }


    }
    public class StringToColor
    {
        public static SolidColorBrush GetSolidColorBrush(string hex)
        {
            hex = hex.Replace("#", string.Empty);
            if(hex.Length == 8) 
            {
                byte a = (byte)(Convert.ToUInt32(hex.Substring(0, 2), 16));
                byte r = (byte)(Convert.ToUInt32(hex.Substring(2, 2), 16));
                byte g = (byte)(Convert.ToUInt32(hex.Substring(4, 2), 16));
                byte b = (byte)(Convert.ToUInt32(hex.Substring(6, 2), 16));
                SolidColorBrush myBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(a, r, g, b));
                return myBrush;
            }
            else if(hex.Length == 6)
            {
                byte r = (byte)(Convert.ToUInt32(hex.Substring(0, 2), 16));
                byte g = (byte)(Convert.ToUInt32(hex.Substring(2, 2), 16));
                byte b = (byte)(Convert.ToUInt32(hex.Substring(4, 2), 16));
                SolidColorBrush myBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(byte.MaxValue, r, g, b));
                return myBrush;
            }
            else if(hex.Length == 3)
            {
                byte g = (byte)(Convert.ToUInt32(hex.Substring(1, 1), 16));
                byte b = (byte)(Convert.ToUInt32(hex.Substring(1, 2), 16));
                SolidColorBrush myBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(byte.MinValue, byte.MaxValue, g, b));
                return myBrush;
            }
            else
            {
                return new SolidColorBrush(Windows.UI.Color.FromArgb(byte.MaxValue, byte.MinValue, byte.MinValue, byte.MinValue));
            }
            //byte a = (byte)(Convert.ToUInt32(hex.Substring(0, 2), 16));

        }
    }

}
