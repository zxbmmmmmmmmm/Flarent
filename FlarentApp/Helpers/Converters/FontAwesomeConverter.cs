using FontAwesome.UWP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace FlarentApp.Helpers.Converters
{
    public class FontAwesomeConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string icon)
            {
                var index = icon.LastIndexOf("fa-");
                if (index == -1)
                    return FontAwesomeIcon.None;
                else
                    return GetEnumByDescription<FontAwesomeIcon>(icon.Remove(0, index + 3));//去除无关内容，获取Icon描述
            }

            return FontAwesomeIcon.None;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
        public static T GetEnumByDescription<T>(string description) where T : Enum//通过IconId获取枚举项
        {
            FieldInfo[] fields = typeof(T).GetFields();
            foreach (FieldInfo field in fields)
            {
                object[] objs = field.GetCustomAttributes(typeof(IconIdAttribute), false);    //获取描述属性
                if (objs.Length > 0 && (objs[0] as IconIdAttribute).Id == description)
                {
                    return (T)field.GetValue(null);
                }
            }

            return (T)fields[46].GetValue(null);
            //throw new ArgumentException(string.Format("{0} 未能找到对应的枚举.", description), "Description");
        }
    }
}
