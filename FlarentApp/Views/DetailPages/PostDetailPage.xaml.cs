using FlarentApp.Helpers;
using FlarumApi;
using FlarumApi.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace FlarentApp.Views.DetailPages
{
    /// <summary>
    /// 用于splitView
    /// </summary>
    public sealed partial class PostDetailPage : Page
    {
        public Post Post;
        public ObservableCollection<Post> Posts = new ObservableCollection<Post>();
        public PostDetailPage()
        {
            this.InitializeComponent();
            //GetPost();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter != null)
            {
                if (e.Parameter is string link)
                    GetPost((string)e.Parameter);
                else if (e.Parameter is int id)
                    GetPost(id);
            }

        }
        private async void GetPost(string link)
        {
            string[] words = link.Split('/');
            var targetDiscussion = words[words.Length - 2];
            var targetNum = words[words.Length - 1];//找到最后一个数字
            LoadingProgressRing.Visibility = Visibility.Visible;
            var posts = await FlarumApiProviders.GetPostsWithLink($"https://{Flarent.Settings.Forum}/api/posts?filter[discussion]={targetDiscussion}&filter[number]={targetNum}", Flarent.Settings.Token);
            Posts = posts.Item1;
            LoadingProgressRing.Visibility = Visibility.Collapsed;
            //Posts = posts.Item1;
            PostsListView.ItemsSource = Posts;
        }
        private async void GetPost(int id)
        {
            var posts = await FlarumApiProviders.GetPostsWithLink($"https://{Flarent.Settings.Forum}/api/posts?filter[id]={id}", Flarent.Settings.Token);
            Posts = posts.Item1;
            //Posts = posts.Item1;
            PostsListView.ItemsSource = Posts;
            LoadingProgressRing.Visibility = Visibility.Collapsed;
        }


    }
}
