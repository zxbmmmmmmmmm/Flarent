using FlarentApp.Helpers;
using FlarentApp.Services;
using FlarentApp.Views.DetailPages;
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
using Windows.UI.Xaml.Navigation;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace FlarentApp.Views.Controls
{
    public sealed partial class DiscussionTemplate : UserControl, INotifyPropertyChanged
    {
        public DiscussionTemplate()
        {
            this.InitializeComponent();
            this.DataContextChanged += (s, e) => Bindings.Update();
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsFirstPost
        {
            get => _isFirstPost;
            set
            {
                if (_isFirstPost != value)
                {
                    _isFirstPost = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFirstPost)));
                }
            }
        }
        private bool _isFirstPost
        {
            get
            {             
                if (Discussion.CommentCount <= 1)
                    return false;
                else
                    return true;
            }
            set { }
        }
        public Discussion Discussion
        {
            get { return (Discussion)GetValue(DiscussionProperty); }
            set { SetValue(DiscussionProperty, value); }
        }
        public static readonly DependencyProperty DiscussionProperty =
           DependencyProperty.Register("Discussion", typeof(Discussion), typeof(PostTemplate), new PropertyMetadata(new Discussion()));

        private void LastPostedUserButton_Click(object sender, RoutedEventArgs e)
        {
            if(Flarent.Settings.ViewUsersInPane)
                NavigationService.OpenInRightPane(typeof(UserDetailPage),Discussion.LastPostedUser.Id);
            else
                NavigationService.Navigate<UserDetailPage>(Discussion.LastPostedUser.Id);
        }

        private void PosterButton_Click(object sender, RoutedEventArgs e)
        {
            if (Flarent.Settings.ViewUsersInPane)
                NavigationService.OpenInRightPane(typeof(UserDetailPage), Discussion.User.Id);
            else
                NavigationService.Navigate<UserDetailPage>(Discussion.User.Id);
        }
    }
}
