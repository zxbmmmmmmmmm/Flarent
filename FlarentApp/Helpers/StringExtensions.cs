using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlarentApp.Helpers
{
    internal class StringExtensions
    {
        /// <summary>
        /// 获取两个字符串中间的字符串
        /// </summary>
        /// <param name="str">要处理的字符串,例ABCD</param>
        /// <param name="str1">第1个字符串,例AB</param>
        /// <param name="str2">第2个字符串,例D</param>
        /// <returns>例返回C</returns>
        public static string GetBetweenStr(string str, string str1, string str2)
        {
            int i1 = str.IndexOf(str1);
            if (i1 < 0) //找不到返回空
            {
                return "";
            }

            int i2 = str.IndexOf(str2, i1 + str1.Length); //从找到的第1个字符串后再去找
            if (i2 < 0) //找不到返回空
            {
                return "";
            }

            return str.Substring(i1 + str1.Length, i2 - i1 - str1.Length);
        }
    }
}
