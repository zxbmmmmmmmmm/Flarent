using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace FlarentApp.Helpers.Converters
{
    internal class BooleanInversionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool boolValue)
            {
                if(boolValue)
                    return false;              
                else
                    return true;
            }

            throw new ArgumentException("parameter must be a bool value!");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is bool boolValue)
            {
                if (boolValue)
                    return true;
                else
                    return false;
            }

            throw new ArgumentException("parameter must be a bool value!");
        }
    }
    internal class BooleanVisibilityInversionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool boolValue)
            {
                if (boolValue)
                    return Visibility.Collapsed;
                else
                    return Visibility.Visible;
            }
            else if(value is Visibility visibility)
            {
                if(visibility == Visibility.Visible)
                    return Visibility.Collapsed;
                else
                    return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
            //throw new ArgumentException("parameter must be a bool value!");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is Visibility visibility)
            {
                if (visibility == Visibility.Visible)
                    return false;
                else
                    return true;
            }

            throw new ArgumentException("parameter must be a Visibility value!");
        }
    }
}
