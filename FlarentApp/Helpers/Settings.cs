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
using ReverseMarkdown.Converters;

namespace FlarentApp.Helpers
{
    public static class Flarent
    {
        public static Settings Settings = new Settings();
    }

    public class Settings : INotifyPropertyChanged
    {
        /// <summary>
        /// 论坛网址
        /// 默认论坛请到Config.cs内更改
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
        /// <summary>
        /// 论坛信息
        /// 此内容将会在首次开启时根据论坛网址更新
        /// 请勿随意更改
        /// </summary>
        public Forum ForumInfo
        {
            get => GetSettingsWithClass("ForumInfo", Default.DefaultForum);
            set
            {
                ApplicationData.Current.LocalSettings.Values["ForumInfo"] = JsonHelper.GetJsonByObject(value);
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// 是否在标题栏展示论坛的Logo，若论坛无Logo则使用应用图标
        /// </summary>
        public bool ShowForumLogo
        {
            get => GetSettings("ShowForumLogo", true);
            set
            {
                ApplicationData.Current.LocalSettings.Values["ShowForumLogo"] = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// 当前已登录的用户ID
        /// 请勿随意更改
        /// </summary>
        public int UserId
        {
            get => GetSettings("UserId", 0);
            set
            {
                ApplicationData.Current.LocalSettings.Values["UserId"] = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// 导航栏大小
        /// </summary>
        public int OpenPaneLegnth
        {
            get => GetSettings("OpenPaneLegnth", 160);
            set
            {
                ApplicationData.Current.LocalSettings.Values["OpenPaneLegnth"] = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// 侧栏大小
        /// </summary>
        public int PaneWidth
        {
            get => GetSettings("PaneWidth", 420);
            set
            {
                ApplicationData.Current.LocalSettings.Values["PaneWidth"] = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// 是否在侧栏查看用户
        /// </summary>
        public bool ViewUsersInPane
        {
            get => GetSettings("ViewUsersInPane", true);
            set
            {
                ApplicationData.Current.LocalSettings.Values["ViewUsersInPane"] = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// token，用于身份验证
        /// 请勿随意更改
        /// </summary>
        public string Token
        {
            get => GetSettings("Token", "");
            set
            {
                ApplicationData.Current.LocalSettings.Values["Token"] = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// 当前已登录的用户信息，将在启动和登录时更新
        /// 请勿随意更改
        /// </summary>
        public string UserInfo
        {
            get => GetSettings("UserInfo", JsonConvert.SerializeObject(new User { DisplayName = "未登录" }));
            set
            {
                ApplicationData.Current.LocalSettings.Values["UserInfo"] = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// 是否打开亚克力
        /// </summary>
        public bool IsAcrylicEnabled
        {
            get => GetSettings("IsAcrylicEnabled",false);
            set
            {
                ApplicationData.Current.LocalSettings.Values["IsAcrylicEnabled"] = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// 开发者模式
        /// </summary>
        public bool DeveloperMode
        {
            get => GetSettings("DeveloperMode", false);
            set
            {
                ApplicationData.Current.LocalSettings.Values["DeveloperMode"] = value;
                OnPropertyChanged();
            }
        }

        public bool IsNotifyEnabled
        {
            get => GetSettings("IsNotifyEnabled", true);
            set
            {
                ApplicationData.Current.LocalSettings.Values["IsNotifyEnabled"] = value;
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
        /// <summary>
        /// 获取设置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
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
