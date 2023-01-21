using FlarentApp.Helpers;
using FlarentApp.Services;
using FlarentApp.Views.Controls;
using FlarentApp.Views.DetailPages;
using FlarumApi;
using FlarumApi.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class NewDiscussionDialog : ContentDialog
    {
        public ObservableCollection<Tag> SelectedTags = new ObservableCollection<Tag>();
        public ObservableCollection<Tag> Tags = new ObservableCollection<Tag>();
        public static NewDiscussionDialog Current;


        public NewDiscussionDialog()
        {
            this.InitializeComponent();
            GetTags();
            Current = this;
        }

        private  async void GetTags()
        {
            Tags = await FlarumApiProviders.GetTags(Flarent.Settings.Forum, Flarent.Settings.Token,true);
            TagsListView.ItemsSource = Tags;
        }

        private void AddTagButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void Submit_Click(object sender, RoutedEventArgs e)
        {
            CreateButton.IsEnabled = false;
            try
            {
                await Submit();
            }
            finally
            {
                CreateButton.IsEnabled = true;
            }

        }

        private async Task Submit()
        {
            string text = string.Empty;
            EditZone.Document.GetText(Windows.UI.Text.TextGetOptions.UseCrlf, out text);
            if (text == "")
            {
                ErrorTextBlock.Text = "请输入内容";
                return;
            }
            if (TitleTextBox.Text == "")
            {
                ErrorTextBlock.Text = "请输入标题";
                return;
            }

            var tags = new List<Dictionary<string, string>>();
            foreach (var item in SelectedTags)
            {
                var tag = new Dictionary<string, string>
                {
                    { "type", "tags" },
                    { "id", item.Id.ToString() }
                };
                tags.Add(tag);
            }
            var data = await FlarumApiProviders.CreateDiscussioinAsync(TitleTextBox.Text, text, $"https://{Flarent.Settings.Forum}/api/discussions", tags, Flarent.Settings.Token);
            if (data.Item2 == "")
            {
                Hide();
                new Toast("发送成功");
                NavigationService.Navigate(typeof(DiscussionDetailPage), (int)data.Item1["data"]["id"]);
            }
            else
            {
                var detail = data.Item1["errors"][0]["detail"].ToString();
                if (detail.Contains("tag"))
                    ErrorTextBlock.Text = $"标签错误:{detail}";
                else
                    ErrorTextBlock.Text = $"{detail}";
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        public void TagsListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var clicked = (Tag)e.ClickedItem;
            var tag = Tags.First(p => p.Id == clicked.Id);
            tag.IsSelected = !tag.IsSelected;
            ResetSelectedTags();
            var mainTagsCount = (SelectedTags.Where(p => p.Position != null ).Count());
            var childTagsCount = (SelectedTags.Where(p => p.Position == null).Count());

            for (int i = 0; i < TagsListView.Items.Count() - 1; i++)
            {
                var container = TagsListView.ContainerFromIndex(i) as ListViewItem;
                if (container == null)
                    continue;
                if (Tags[i].IsSelected)               
                    container.Visibility = Visibility.Visible;               
                else
                {
                    if (Tags[i].Position != null&& mainTagsCount >= Flarent.Settings.ForumInfo.MaxPrimaryTags)//主标签限制
                        container.Visibility = Visibility.Collapsed;                   
                    else if(Tags[i].Position == null && childTagsCount >= Flarent.Settings.ForumInfo.MaxSecondaryTags)//副标签限制
                        container.Visibility = Visibility.Collapsed;
                    else
                        container.Visibility = Visibility.Visible;
                }

            }


        }
        public void ResetSelectedTags()
        {
            SelectedTags.Clear();
            var list = Tags.Where(p => p.IsSelected == true).ToList();
            foreach (var item in list)
            {
                SelectedTags.Add(item);
                foreach (var child in item.Chidren)
                {
                    if(child.IsSelected)
                        SelectedTags.Add(child);
                }
            }

        }
    }
}
