using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Tasks.Helpers
{
    internal class SettingsHelper
    {
        public static string Forum
        {
            get => ApplicationData.Current.LocalSettings.Values["Forum"].ToString();
        }
        public static string Token
        {
            get => ApplicationData.Current.LocalSettings.Values["Token"].ToString();
        }
        public static bool IsNotifyEnabled
        {
            get => (bool)ApplicationData.Current.LocalSettings.Values["IsNotifyEnabled"];
        }
    }
}
