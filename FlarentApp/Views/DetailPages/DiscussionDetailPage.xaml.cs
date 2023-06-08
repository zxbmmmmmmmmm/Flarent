using FlarentApp.Helpers;
using FlarentApp.Services;
using FlarentApp.Views.Dialogs;
using FlarentApp.Views.WindowPages;
using FlarumApi;
using FlarumApi.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.WindowManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
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
    public sealed partial class DiscussionDetailPage : Page, INotifyPropertyChanged,IDisposable
    {
        public ScrollViewer PostScrollViewer = new ScrollViewer();
        //public Discussion Discussion= new Discussion();
        public List<int> PostIds = new List<int>();

        public ObservableCollection<Post> Posts { get; set; } = new ObservableCollection<Post>();
        //public int CurrentPage;
        public int LastPosts;
        public int TotalPages;
        public int discussionId;
        // bool isLastPage = false;
        public Discussion Discussion
        {
            get => _discussion;
            set
            {
                if (_discussion != value)
                {
                    _discussion = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Discussion)));
                }
            }
        }
        private Discussion _discussion;
        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (_currentPage != value)
                {
                    _currentPage = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentPage)));
                }
            }
        }
        private int _currentPage;

        public bool IsLastPage
        {
            get => _isLastPage;
            set
            {
                if (_isLastPage != value)
                {
                    _isLastPage = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLastPage)));
                }
            }
        }
        private bool _isLastPage;
        public bool IsFirstPage
        {
            get => _isFirstPage;
            set
            {
                if (_isFirstPage != value)
                {
                    _isFirstPage = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFirstPage)));
                }
            }
        }
        private bool _isFirstPage;
        private bool disposedValue;

        public event PropertyChangedEventHandler PropertyChanged;

        public DiscussionDetailPage()
        {
            this.InitializeComponent();
            RequestedTheme = (Window.Current.Content as FrameworkElement).RequestedTheme;
            PostSlider.AddHandler(UIElement.PointerReleasedEvent /*哪个事件*/, new PointerEventHandler(PostSlider_PointerReleased) /*使用哪个函数处理*/, true /*如果在之前处理，是否还使用函数*/);

        }
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var anim = ConnectedAnimationService.GetForCurrentView().GetAnimation("ForwardConnectedAnimation");
            if (anim != null)
            {
                anim.TryStart(this);
            }

            if (e.Parameter != null)
            {
                discussionId = (int)e.Parameter;
            }
            await GetDiscussion();
            await TurnToPage(0);
            
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            PostScrollViewer.ViewChanged -= PostScrollViewer_ViewChanged;
            PostSlider.PointerReleased -= PostSlider_PointerReleased;
            NextPageButton.Click -= NextPageButton_Click;
            PreviousPageButton.Click -= PreviousPageButton_Click;
            ReplyButton.Click -= ReplyButton_Click;
            DownloadItem.Click -= DownloadItem_Click;
            RefreshItem.Click -= RefreshItem_Click;
            PostsListView.Loaded -= PostsListView_Loaded;
            ReadModeButton.Click -= ReadModeButton_Click;
            GC.Collect();
            Dispose();
        }
        private void PostScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var value = PostScrollViewer.VerticalOffset / PostScrollViewer.ExtentHeight;//百分比
            if (IsLastPage && LastPosts != 0)
            {
                var postsLeft = PostIds.Count % 30;
                PostSlider.Value = CurrentPage * 30 + value * (postsLeft + 10);
            }
            else
            {
                PostSlider.Value = CurrentPage * 30 + (value + 0.025) * 30;
            }
        }
        private async void PostSlider_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            //假设CurrentPage = 1
            var sliderValue = ((Slider)sender).Value;//获取到slider的值（159）
            double sliderValue_ = (sliderValue / 30); //将slider值除以30，可以得到（5.3333）          
            //IsLastPage = TotalPages <= Math.Floor(sliderValue_);//获取是否为最后一页
            if (!(CurrentPage < sliderValue_ && sliderValue_ < CurrentPage + 1))
            {
                CurrentPage = (int)Math.Floor(sliderValue_);//取到整数(5)并设定为页码
                await TurnToPage(CurrentPage);
            }
            double scrollValue = 0;
            //(更改scrollviewer值)
            if (IsLastPage&&LastPosts!=0)
            {
                scrollValue = (sliderValue % 30 / LastPosts) * PostScrollViewer.ExtentHeight;
            }
            else
            {
                scrollValue = (sliderValue % 30 / 30) * PostScrollViewer.ExtentHeight;
            }
            PostScrollViewer.ChangeView(PostScrollViewer.HorizontalOffset, scrollValue, PostScrollViewer.ZoomFactor);//导航到需要的值
        }
        /// <summary>
        /// 获取讨论信息
        /// </summary>
        public async Task GetDiscussion()
        {
            PostsListView.Visibility = Visibility.Collapsed;
            LoadingProgressRing.Visibility = Visibility.Visible;
            ReplyButton.IsEnabled = false;
            PostSlider.IsEnabled = false;
            Discussion = await FlarumApiProviders.GetDiscussion(discussionId, 0, Flarent.Settings.Forum,Flarent.Settings.Token);
            PostSlider.Maximum = Discussion.Posts.Count;
            PostIds = new List<int>();
            foreach (var item in Discussion.Posts)//获取帖子列表
            {
                PostIds.Add((int)item.Id);
            }
            CurrentPage = 0;
            TotalPages = (int)Math.Ceiling((double)Discussion.Posts.Count / 30);
            LastPosts = PostIds.Count % 30;//将post的数量取余30，得到剩余的post数量（29）
            PostsListView.Visibility = Visibility.Visible;
            ReplyButton.IsEnabled = true;
            LoadingProgressRing.Visibility = Visibility.Collapsed;
            if (TotalPages < 2)
                SliderPanel.Visibility = Visibility.Collapsed;
        }
        private async Task GetPost(int min, int range)
        {
            PostSlider.IsEnabled = false;
            var list = PostIds.GetRange(min, range);
            var data = await FlarumApiProviders.GetPostsWithId(list, Flarent.Settings.Forum, Flarent.Settings.Token);
            if (data[0].Number == 1 && Discussion.User == null)
            {
                Discussion.User = data[0].User;
            }
            Posts.Clear();
            foreach (var post in data)
            {
                Posts.Add(post);
            }
            PostsListView.ItemsSource = Posts;
            PageTextBlock.Text = $"{CurrentPage + 1}/{TotalPages}页";
            PagePostsTextBlock.Text = $"第{min + 1}-{min + range}条回复";
            PostSlider.IsEnabled = true;

        }
        public static ScrollViewer GetScrollViewer(DependencyObject parent)
        {
            if (parent == null)
                return null;
            var count = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < count;)
            {
                var item = VisualTreeHelper.GetChild(parent, i);
                if (item is ScrollViewer viewer)
                {
                    return viewer;
                }
                else
                {
                    return GetScrollViewer(item);
                }
            }
            return null;
        }

        private async void NextPageButton_Click(object sender, RoutedEventArgs e)
        {
            //IsLastPage = CurrentPage + 2 >= TotalPages;
            //if (IsLastPage)  
            //PostSlider.Value = (CurrentPage + 1) * 30;          
            //else
            //PostSlider.Value = (CurrentPage + 1) * 30;

            await TurnToPage(CurrentPage + 1);
        }

        private async Task TurnToPage(int page)
        {
            CurrentPage = page;

            IsFirstPage = CurrentPage == 0;

            IsLastPage = TotalPages <= CurrentPage + 1;
            var min = CurrentPage * 30;//最小值
            var range = 30;
            if (IsLastPage&&LastPosts!=0)
            {
                range = LastPosts;
            }
            await GetPost(min, range);
            if (PostScrollViewer == null)
            {
                PostScrollViewer = GetScrollViewer(PostsListView);
                PostScrollViewer.ViewChanged -= PostScrollViewer_ViewChanged;
                PostScrollViewer.ViewChanged += PostScrollViewer_ViewChanged;
            }


        }

        private async void PreviousPageButton_Click(object sender, RoutedEventArgs e)
        {
            await TurnToPage(CurrentPage - 1);
            PostScrollViewer.ChangeView(PostScrollViewer.HorizontalOffset, PostScrollViewer.ExtentHeight, PostScrollViewer.ZoomFactor);//导航到底部
        }
       
        private async void ReplyButton_Click(object sender, RoutedEventArgs e)
        {
            //var dialog = new ReplyDialog(Discussion).ShowAsync();
            await WindowService.Current.CreateReplyWindow(Discussion);
        }

        private async void DownloadItem_Click(object sender, RoutedEventArgs e)
        {
            var tuple = new Tuple<List<int>, Discussion>(PostIds, Discussion);
            await new DownloadDialog(PostIds, Discussion).ShowAsync();
        }
        public async void TurnToLastPage()
        {
            await TurnToPage(TotalPages - 1);
            PostScrollViewer.ChangeView(PostScrollViewer.HorizontalOffset, PostScrollViewer.ExtentHeight, PostScrollViewer.ZoomFactor);//导航到底部
        }

        private async void RefreshItem_Click(object sender, RoutedEventArgs e)
        {
            await GetDiscussion();
            await TurnToPage(0);
        }

        private void PostsListView_Loaded(object sender, RoutedEventArgs e)
        {
            //PostScrollViewer = (VisualTreeHelper.GetChild(/*ListView控件*/, 0) as Border).Child as ScrollViewer;
            //Border border = VisualTreeHelper.GetChild(PostsListView, 0) as Border;
            //ScrollViewer scrollViewer = VisualTreeHelper.GetChild(border, 0) as ScrollViewer;
            PostScrollViewer = GetScrollViewer(PostsListView);
            if(PostScrollViewer!= null)
            {
                PostScrollViewer.ViewChanged -= PostScrollViewer_ViewChanged;
                PostScrollViewer.ViewChanged += PostScrollViewer_ViewChanged;
            }

        }

        private void ReadModeButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(typeof(ReadModePage),Discussion);
        }

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                Discussion = null;
                PostIds = null;
                Posts = null;              
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~DiscussionDetailPage()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

}
