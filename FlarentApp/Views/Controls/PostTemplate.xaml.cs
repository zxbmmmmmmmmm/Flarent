using FlarentApp.Helpers;
using FlarentApp.Helpers.Converters;
using FlarentApp.Services;
using FlarentApp.Views.DetailPages;
using FlarentApp.Views.Dialogs;
using FlarumApi.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI;
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
    public sealed partial class PostTemplate : UserControl, INotifyPropertyChanged
    {
        public PostTemplate()
        {
            this.InitializeComponent();
            ContentMarkdownTextBlock.Background = new SolidColorBrush(Colors.Transparent);
            DescriptionTextBlock.Background = new SolidColorBrush(Colors.Transparent);

            this.DataContextChanged += (s, e) => Bindings.Update();
            Loaded += UserControl_Loaded;
            Unloaded += UserControl_Unloaded;
        }

        private async void EditMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var converter = new HtmlToMarkdownConverter();

            var text = (string)converter.Convert(Post.ContentHtml, null, null
                , null);
            await new ReplyDialog(null, Post, text,$"https://{Flarent.Settings.Forum}/d/{Post.Discussion.Id}/{Post.Number}").ShowAsync();
        }

        private void VotesToggleButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void ReplyButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var clicked = btn.DataContext as Post;
            await new ReplyDialog(Post.Discussion,null, $"@\"{clicked.User.DisplayName}\"#p{clicked.Id} ").ShowAsync();
        }

        private void WindowSizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            if(e.Size.Width>=865&&e.Size.Width>=550&&CanAdaptive)
            {
                PostArea.Margin = new Thickness(32, 4, 200, 0);
            }
            else
            {
                PostArea.Margin = new Thickness(32, 0, 32, 0);
            }
        }


        public Post Post
        {
            get { return (Post)GetValue(PostProperty); }
            set { SetValue(PostProperty, value); }
        }
        public static readonly DependencyProperty PostProperty =
           DependencyProperty.Register("Post", typeof(Post), typeof(PostTemplate), new PropertyMetadata(new Post()));
        /// <summary>
        /// 是否允许自适应布局
        /// </summary>
        public bool CanAdaptive
        {
            get { return (bool)GetValue(CanAdaptiveProperty); }
            set { SetValue(CanAdaptiveProperty, value); }
        }
        public static readonly DependencyProperty CanAdaptiveProperty =
           DependencyProperty.Register("CanAdaptive", typeof(bool), typeof(PostTemplate), new PropertyMetadata(false));

        public event PropertyChangedEventHandler PropertyChanged;

        private void UserButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = (HyperlinkButton)sender;
            var post = (Post)btn.DataContext;
            if (Flarent.Settings.ViewUsersInPane)
                NavigationService.OpenInRightPane(typeof(UserDetailPage), post.User.Id);
            else
                NavigationService.Navigate<UserDetailPage>(post.User.Id);
        }

        private async void ContentMarkdownTextBlock_LinkClicked(object sender, Microsoft.Toolkit.Uwp.UI.Controls.LinkClickedEventArgs e)
        {
            var link = e.Link;
            if (link.Contains($"{Flarent.Settings.Forum.ToLower()}/d/"))
            {
                var split = link.Split("/");
                var id = split[split.Count() - 1];
                if(split.Count() > 5)
                    NavigationService.OpenInRightPane(typeof(PostDetailPage), link);
                else
                {
                    if(id.Contains("-"))//防止链接后面有字符存在
                    {
                        var index = id.IndexOf('-');
                        id = id.Remove(index);                        
                    }
                    NavigationService.Navigate<DiscussionDetailPage>(int.Parse(id));
                }

            }
            else if(link.Contains($"{Flarent.Settings.Forum.ToLower()}/u/"))
            {
                var split = link.Split("/");
                var uid = split[split.Count() - 1];
                if (Flarent.Settings.ViewUsersInPane)
                    NavigationService.OpenInRightPane(typeof(UserDetailPage), uid);
                else
                    NavigationService.Navigate<UserDetailPage>(uid);
            }
            else
                await Launcher.LaunchUriAsync(new Uri(link));
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            Window.Current.SizeChanged += WindowSizeChanged;
            ReplyButton.Click -= ReplyButton_Click;
            ReplyButton.Click += ReplyButton_Click;
            VotesToggleButton.Click -= VotesToggleButton_Click;
            VotesToggleButton.Click += VotesToggleButton_Click;
            EditMenuItem.Click -= EditMenuItem_Click;
            EditMenuItem.Click += EditMenuItem_Click;
            if (Window.Current.Bounds.Width >= 865 && Window.Current.Bounds.Width >= 550 && CanAdaptive)
            {
                PostArea.Margin = new Thickness(32, 4, 200, 0);
            }
            else
            {
                PostArea.Margin = new Thickness(32,0,32,0);
            }
        }
        /// <summary>
        /// 回收控件防止内存泄露
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
           // UserButton.Click -= UserButton_Click;
            //ContentMarkdownTextBlock.LinkClicked -= ContentMarkdownTextBlock_LinkClicked;
            Window.Current.SizeChanged -= WindowSizeChanged;
            //ReplyButton.Click -= ReplyButton_Click;
            //VotesToggleButton.Click -= VotesToggleButton_Click;
            //EditMenuItem.Click -= EditMenuItem_Click;         
        }

    }
}
