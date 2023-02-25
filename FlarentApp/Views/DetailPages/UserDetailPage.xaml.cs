using FlarentApp.Helpers;
using FlarentApp.Views.Controls;
using FlarumApi;
using FlarumApi.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace FlarentApp.Views.DetailPages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class UserDetailPage : Page, INotifyPropertyChanged
    {
        public int UserId = 1;
        public User User
        {
            get => _user;
            set
            {
                if (_user != value)
                {
                    _user = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(User)));
                }
            }
        }
        private User _user;

        public event PropertyChangedEventHandler PropertyChanged;

        public UserDetailPage()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter != null)
            {
                if (e.Parameter is int uid)
                    GetUser($"https://{Flarent.Settings.Forum}/api/users/{uid.ToString()}");
                else if(e.Parameter is string userName)
                    GetUser($"https://{Flarent.Settings.Forum}/api/users/{userName}?bySlug=true");
            }
        }
        public async void GetUser(string link)
        {
            User = await FlarumApiProviders.GetUser(link,Flarent.Settings.Token);
            UserContentFrame.Navigate(typeof(PostsPage), User.UserName);
        }

        private void UserNavigationView_ItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
        {
            var item = sender.SelectedItem as Microsoft.UI.Xaml.Controls.NavigationViewItem;
            switch (item.Name)
            {
                case "PostsItem":
                    UserContentFrame.Navigate(typeof(PostsPage), User.UserName);
                    break;
                case "DiscussionsItem":
                    UserContentFrame.Navigate(typeof(DiscussionsPage), User.UserName);
                    break;
            }
        }

        private void AvatarEllipse_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("ForwardConnectedAnimation", (UIElement)sender);
            new ImageView().Show(User.AvatarUrl);
        }
    }
}
