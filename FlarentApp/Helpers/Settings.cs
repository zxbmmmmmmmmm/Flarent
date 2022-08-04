using FlarumApi;
using FlarumApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI.Core;
using FlarumApi.Helpers;

namespace FlarentApp.Helpers
{
    internal static class Flarent
    {
        public static Settings Settings = new Settings();
    }

    public class Settings : INotifyPropertyChanged
    {
        /// <summary>
        /// 默认论坛。你可以将discuss.flarum.org换为其他论坛以便制作专属客户端
        /// </summary>
        public string Forum
        {
            get => GetSettings("Forum", Config.Forum);
            set
            {
                ApplicationData.Current.LocalSettings.Values["Forum"] = value;
                OnPropertyChanged();
            }
        }

        public Forum ForumInfo
        {
            get => GetSettingsWithClass("ForumInfo", Default.DefaultForum);
            set
            {
                ApplicationData.Current.LocalSettings.Values["ForumInfo"] = JsonHelper.GetJsonByObject(value);
                OnPropertyChanged();
            }
        }
        public bool ShowForumLogo
        {
            get => GetSettings("ShowForumLogo", true);
            set
            {
                ApplicationData.Current.LocalSettings.Values["ShowForumLogo"] = value;
                OnPropertyChanged();
            }
        }
        public int UserId
        {
            get => GetSettings("UserId", 0);
            set
            {
                ApplicationData.Current.LocalSettings.Values["UserId"] = value;
                OnPropertyChanged();
            }
        }
        public int OpenPaneLegnth
        {
            get => GetSettings("OpenPaneLegnth", 160);
            set
            {
                ApplicationData.Current.LocalSettings.Values["OpenPaneLegnth"] = value;
                OnPropertyChanged();
            }
        }
        public int PaneWidth
        {
            get => GetSettings("PaneWidth", 420);
            set
            {
                ApplicationData.Current.LocalSettings.Values["PaneWidth"] = value;
                OnPropertyChanged();
            }
        }
        public bool ViewUsersInPane
        {
            get => GetSettings("ViewUsersInPane", true);
            set
            {
                ApplicationData.Current.LocalSettings.Values["ViewUsersInPane"] = value;
                OnPropertyChanged();
            }
        }
        public string Token
        {
            get => GetSettings("Token", "");
            set
            {
                ApplicationData.Current.LocalSettings.Values["Token"] = value;
                OnPropertyChanged();
            }
        }
        public string UserInfo
        {
            get => GetSettings("UserInfo", JsonConvert.SerializeObject(new User { DisplayName = "未登录" }));
            set
            {
                ApplicationData.Current.LocalSettings.Values["UserInfo"] = value;
                OnPropertyChanged();
            }
        }
        public bool IsAcrylicEnabled
        {
            get => GetSettings("IsAcrylicEnabled",false);
            set
            {
                ApplicationData.Current.LocalSettings.Values["IsAcrylicEnabled"] = value;
                OnPropertyChanged();
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public async void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () => { PropertyChanged(this, new PropertyChangedEventArgs(propertyName)); });
        }
        public static T GetSettings<T>(string propertyName, T defaultValue)
        {
            try
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey(propertyName) &&
                    ApplicationData.Current.LocalSettings.Values[propertyName] != null &&
                    !string.IsNullOrEmpty(ApplicationData.Current.LocalSettings.Values[propertyName].ToString()))
                {
                    if (typeof(T).ToString() == "System.Boolean")
                        return (T)(object)bool.Parse(ApplicationData.Current.LocalSettings.Values[propertyName]
                            .ToString());

                    return (T)ApplicationData.Current.LocalSettings.Values[propertyName];
                }
                else
                {
                    ApplicationData.Current.LocalSettings.Values[propertyName] = defaultValue;
                }
                return defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }
        /// <summary>
        /// 将设置转换为Json后保存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T GetSettingsWithClass<T>(string propertyName, T defaultValue)//使用default value中的T
        {
            try
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey(propertyName) &&
                    ApplicationData.Current.LocalSettings.Values[propertyName] != null &&
                    !string.IsNullOrEmpty(ApplicationData.Current.LocalSettings.Values[propertyName].ToString()))
                {
                    if (typeof(T).ToString() == "System.Boolean")
                        return (T)(object)bool.Parse(ApplicationData.Current.LocalSettings.Values[propertyName]
                            .ToString());
                    var str = (string)ApplicationData.Current.LocalSettings.Values[propertyName];//获取字符串
                    return (T)JsonHelper.GetObjectByJson<T>(str);
                }
                else
                {
                    ApplicationData.Current.LocalSettings.Values[propertyName] = JsonHelper.GetJsonByObject(defaultValue);
                }
                return defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }
    }
    public class Default
    {
        public static Forum DefaultForum
        {
            get
            {
                return new Forum
                {
                    Name = Flarent.Settings.Forum,
                    Website = Flarent.Settings.Forum,
                    BaseUrl = $"https://{Flarent.Settings.Forum}",
                    FavIcon = "ms-appx:///Assets/StoreLogo.png",
                    Logo = "ms-appx:///Assets/StoreLogo.png"
                };

            }
        }

    }
}
