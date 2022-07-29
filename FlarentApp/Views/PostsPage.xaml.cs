using FlarentApp.Helpers;
using FlarentApp.Services;
using FlarentApp.Views.DetailPages;
using FlarumApi;
using FlarumApi.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace FlarentApp.Views
{
    public sealed partial class PostsPage : Page, INotifyPropertyChanged
    {
        public ObservableCollection<Post> Posts = new ObservableCollection<Post>();
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


        //private string _linkNext = $"https://{Flarent.Settings.Forum}/api/posts?sort=-createdAt&filter[type]=discussionRenamed";
        private string _linkNext = $"https://{Flarent.Settings.Forum}/api/posts?sort=-createdAt";

        public int User
        {
            get { return (int)GetValue(ShowTextProperty); }
            set { SetValue(ShowTextProperty, value); }
        }

        public static readonly DependencyProperty ShowTextProperty =
            DependencyProperty.Register("User", typeof(int), typeof(PostsPage), new PropertyMetadata(0));
        public PostsPage()
        {
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
            InitializeComponent();
        }
        /*
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (User != 0)
            {
                LinkNext = $"https://{Flarent.Settings.Forum}/api/posts?sort=-createdAt&filter[type]=comment&filter[user]={User}";
            }
            GetPosts();

        }*/
        
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if(e.NavigationMode != NavigationMode.Back)
            {
                base.OnNavigatedTo(e);
                LoadingProgressRing.Visibility = Visibility.Visible;

                if (e.Parameter != null)
                {
                    if (e.Parameter is string username)
                    {
                        //LinkNext = $"https://{Flarent.Settings.Forum}/api/posts?sort=-createdAt&filter[type]=comment&page[limit]=10&filter[author]={username}";
                        LinkNext = $"https://{Flarent.Settings.Forum}/api/posts?sort=-createdAt&page[limit]=10&filter[author]={username}";
                        Posts.Clear();
                        GetPosts();
                        return;
                    }
                    return;
                }
                Posts.Clear();
                GetPosts();
            }


        }
        private async void GetPosts()
        {
            try
            {
                ErrorControl.Visibility = Visibility.Collapsed;
                LoadMoreButton.IsEnabled = false;
                var data = await FlarumApiProviders.GetPostsWithLink(LinkNext, Flarent.Settings.Token);
                var posts = data.Item1;
                LinkNext = data.Item2;
                foreach (var post in posts)
                    Posts.Add(post);
                PostsListView.ItemsSource = Posts;
                LoadMoreButton.IsEnabled = true;
            }
            catch
            {
                ErrorControl.Visibility = Visibility.Visible;
                LoadMoreButton.Visibility = Visibility.Collapsed;
            }
            finally
            {
                LoadingProgressRing.Visibility = Visibility.Collapsed;
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void LoadMoreButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            GetPosts();
        }



        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            LoadMoreButton.Click -= LoadMoreButton_Click;
        }

        private void LinkTextBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            Posts.Clear();
            LinkNext = sender.Text;
            GetPosts();
        }
    }
}
