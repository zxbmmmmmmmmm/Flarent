using FlarentApp.Helpers;
using FlarentApp.Services;
using FlarumApi;
using FlarumApi.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace FlarentApp.Views.DetailPages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class DiscussionsPage : Page, INotifyPropertyChanged
    {
        public string LinkNext
        {
            get => _linkNext;
            set
            {
                if (_linkNext != value)
                {
                    _linkNext = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LinkNext)));
                }
            }
        }
        private string _linkNext = $"https://{Flarent.Settings.Forum}/api/posts?sort=-createdAt";
        public ObservableCollection<Discussion> Discussions = new ObservableCollection<Discussion>();

        public event PropertyChangedEventHandler PropertyChanged;

        public DiscussionsPage()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter != null)
            {
                if (e.Parameter is string username)
                {
                    LinkNext = $"https://{Flarent.Settings.Forum}/api/discussions?sort=-createdAt&filter[author]={username}";
                    GetDiscussions();
                    return;
                }
                return;
            }
            GetDiscussions();
        }
        private async void GetDiscussions()
        {
            LoadMoreButton.IsEnabled = false;
            var data = await FlarumApiProviders.GetDiscussions(null,LinkNext, null,Flarent.Settings.Token);
            var discussions = data.Item1;
            LinkNext = data.Item2;
            foreach (var post in discussions)
                Discussions.Add(post);
            DiscussionsListView.ItemsSource = Discussions;
            LoadMoreButton.IsEnabled = true;
            LoadingProgressRing.Visibility = Visibility.Collapsed;
        }

        private void LoadMoreButton_Click(object sender, RoutedEventArgs e)
        {
            GetDiscussions();
        }

        private void DiscussionsListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var clicked = e.ClickedItem as Discussion;
            var id = clicked.Id;
            NavigationService.Navigate<DiscussionDetailPage>(id);
        }
    }
}
