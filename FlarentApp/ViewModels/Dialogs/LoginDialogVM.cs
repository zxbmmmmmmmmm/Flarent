using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlarentApp.Helpers;
using FlarentApp.Views.Controls;
using FlarumApi;
using Windows.System;
using Windows.UI.Xaml.Controls;

namespace FlarentApp.ViewModels.Dialogs
{
    public sealed partial class LoginDialogVM:ObservableObject
    {
        public ContentDialog Dialog;
        [RelayCommand]
        public async Task Login()
        {;
            var forum = Flarent.Settings.Forum;
            var tup = await FlarumApiProviders.GetToken(Username, Password, forum, IsRemember);
            if (tup.Item3 == "")
            {
                Flarent.Settings.UserId = tup.Item1;
                Flarent.Settings.Token = tup.Item2;
                new Toast("登录成功").Show();
                Close();
            }
            else
            {
                switch (tup.Item3)
                {
                    case "not_authenticated":
                        Error = "用户名或密码错误";
                        break;
                    default:
                        Error = "出现错误，请重试";
                        break;
                }
            }
        }
        [RelayCommand]
        private void Close()
        {
            Dialog.Hide();
        }
        [RelayCommand]
        private async void SignUp()
        {
            await Launcher.LaunchUriAsync(new Uri($"https://{Flarent.Settings.Forum}"));
        }
        [ObservableProperty]
        private bool isRemember = true;
        [ObservableProperty]
        private string error;
        [ObservableProperty]
        private string username;
        [ObservableProperty]
        private string password;
        partial void OnUsernameChanged(string value)
        {
            Error = "";
        }
        partial void OnPasswordChanged(string value)
        {
            Error = "";
        }
    }
}
