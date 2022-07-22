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

namespace FlarentApp.Helpers
{
    internal static class Flarent
    {
        public static Settings Settings = new Settings();
    }

    public class Settings : INotifyPropertyChanged
    {
        public string Forum
        {
            get => GetSettings("Forum", "discuss.flarum.org.cn");
            set
            {
                ApplicationData.Current.LocalSettings.Values["Forum"] = value;
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

                    //超长的IF
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

    }

}
