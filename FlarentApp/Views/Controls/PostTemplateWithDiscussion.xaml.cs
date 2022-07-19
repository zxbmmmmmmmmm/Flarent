using FlarentApp.Services;
using FlarentApp.Views.DetailPages;
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
    public sealed partial class PostTemplateWithDiscussion : UserControl
    {
        public PostTemplateWithDiscussion()
        {
            this.InitializeComponent();
            this.DataContextChanged += (s, e) => Bindings.Update();
            Unloaded -= UserControl_Unloaded;
            Unloaded += UserControl_Unloaded;
            DiscussionButton.Click -= DiscussionButton_Click;
            DiscussionButton.Click += DiscussionButton_Click;
        }
        public Post Post
        {
            get { return (Post)GetValue(PostProperty); }
            set { SetValue(PostProperty, value); }
        }
        public static readonly DependencyProperty PostProperty =
           DependencyProperty.Register("Post", typeof(Post), typeof(PostTemplate), new PropertyMetadata(new Post()));

        private void DiscussionButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            NavigationService.Navigate<DiscussionDetailPage>(Post.Discussion.Id);
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            DiscussionButton.Click -= DiscussionButton_Click;
        }
    }
}
