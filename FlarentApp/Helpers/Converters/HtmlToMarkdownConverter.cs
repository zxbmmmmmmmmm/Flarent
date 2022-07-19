using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace FlarentApp.Helpers.Converters
{
    internal class HtmlToMarkdownConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return null;
            }
            else
            {
                var converter = new ReverseMarkdown.Converter();
                var str = converter.Convert(value.ToString());
                str = str.Replace("<del>", "~~");//删除线替换
                str = str.Replace("</del>", "~~");//删除线替换
                return str;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
