using FlarentApp.Services;
using FlarentApp.Views.DetailPages;
using FlarumApi;
using FlarumApi.Models;
using System;
using System.Collections.Generic;
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
using Windows.UI.Xaml.Navigation;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace FlarentApp.Views.Controls
{
    public sealed partial class UserHyperLinkButton : UserControl
    {
        public UserHyperLinkButton()
        {
            this.InitializeComponent();
            this.DataContextChanged += (s, e) => Bindings.Update();
            UserHyperLinkBtn.Click -= UserHyperLinkButton_Click;
            UserHyperLinkBtn.Click += UserHyperLinkButton_Click;
        }

        private void UserHyperLinkButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.OpenInRightPane(typeof(UserDetailPage), UserData.Id);
        }

        public User UserData
        {
            get { return (User)GetValue(UserDataProperty); }
            set { SetValue(UserDataProperty, value); }
        }

        public static readonly DependencyProperty UserDataProperty =
            DependencyProperty.Register("UserData", typeof(User), typeof(UserTemplate), new PropertyMetadata(Preset.DefaultUser));

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            UserHyperLinkBtn.Click -= UserHyperLinkButton_Click;
        }
    }
}
