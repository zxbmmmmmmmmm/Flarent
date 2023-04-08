using FlarentApp.Helpers;
using FlarumApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“内容对话框”项模板

namespace FlarentApp.Views.Dialogs
{
    public sealed partial class LoginDialog : ContentDialog
    {
        public LoginDialog()
        {
            RequestedTheme = (Window.Current.Content as FrameworkElement).RequestedTheme;
            this.InitializeComponent();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var userName = UserNameTextBox.Text;
            var password = MyPasswordBox.Password;
            var forum = Flarent.Settings.Forum;
            var isRemembered = RememberMeCheckBox.IsChecked;
            var tup = await FlarumApiProviders.GetToken(userName, password, forum, (bool)isRemembered);
            if(tup.Item3 == "")
            {
                Flarent.Settings.UserId = tup.Item1;
                Flarent.Settings.Token = tup.Item2;
                Hide();
            }
            else
            {
                switch (tup.Item3)
                {
                    case "not_authenticated":
                        ErrorTextBlock.Text = "用户名或密码错误";
                        ErrorTextBlock.Visibility = Visibility.Visible;
                        break;
                }

            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void UserNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ErrorTextBlock.Visibility = Visibility.Collapsed;
        }

        private void MyPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ErrorTextBlock.Visibility = Visibility.Collapsed;
        }

        private void UserNameTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter) MyPasswordBox.Focus(FocusState.Keyboard);

        }

        private void MyPasswordBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter) LoginButton_Click(null, null);
        }

        private  async void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri($"https://{Flarent.Settings.Forum}"));
        }
    }
}
