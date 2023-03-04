using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks.Helpers
{
    internal class DateHelper
    {
        public static string FriendFormat(DateTime dateTime)
        {
            if (dateTime == null)
            {
                return string.Empty;
            }
            try
            {
                var now = DateTime.Now.Ticks;
                var tick = dateTime.Ticks;
                var diff_ = now - tick;
                var diffDt = new DateTime(diff_);
                if (diffDt.Year <= 1 && diffDt.Month < 4)
                {
                    if (diffDt.Month <= 1)
                    {
                        if (diffDt.Day <= 1)
                        {
                            if (diffDt.Hour < 1)
                            {
                                if (diffDt.Minute < 2)
                                {
                                    return $"刚刚";
                                }
                                else
                                {
                                    return $"{diffDt.Minute}分钟前";
                                }
                            }
                            else
                            {
                                return $"{diffDt.Hour}小时前";
                            }
                        }
                        else
                        {
                            return $"{diffDt.Day}天前";
                        }
                    }
                    else
                    {
                        return $"{diffDt.Month - 1}个月前";
                    }
                }
                else
                {
                    return dateTime.ToString("yyyy-MM-dd");
                }
            }

            catch
            {
                return dateTime.ToString("yyyy-MM-dd");
            }

            /*int nowYear = DateTime.Now.Year;
            int year = dateTime.Year;
            if (nowYear - year == 0)
            {
                //本年内
                int nowMounth = DateTime.Now.Month;
                int mounth = dateTime.Month;
                if (nowMounth - mounth == 0)
                {
                    //本月内
                    int nowDay = DateTime.Now.Day;
                    int day = dateTime.Day;
                    int diffDay = nowDay - day;

                    if (nowDay - day == 0)
                    {
                        //当天
                        int nowHour = DateTime.Now.Hour;
                        int hour = dateTime.Hour;
                        int diffHour = nowHour - hour;

                        if (nowHour - hour == 0)
                        {
                            int nowMinute = DateTime.Now.Minute;
                            int minute = dateTime.Minute;
                            int diff = nowMinute - minute;
                            if (diff < 2)
                            {
                                return "刚刚";
                            }
                            else
                            {
                                return $"{diff} 分钟前";
                            }
                        }
                        else
                        {

                            return $"{diffHour} 小时前";
                        }
                    }
                    else
                    {
                        return $"{diffDay} 天前";
                    }
                }
                else
                {
                    //跨月
                    return dateTime.ToString("yyyy-MM-dd");
                }
            }
            else
            {
                //跨年
                return dateTime.ToString("yyyy-MM-dd"); // "yyyy-MM-dd HH:mm:ss"
            }
        }*/
        }

    }
}
