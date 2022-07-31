using FlarentApp.Helpers;
using FlarumApi;
using FlarumApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class DownloadDialog : ContentDialog
    {
        public List<int> PostIds = new List<int>();
        public List<Post> Posts = new List<Post>();
        public Discussion Discussion = new Discussion();
        public DownloadDialog(List<int> postIds,Discussion discussion)
        {
            RequestedTheme = (Window.Current.Content as FrameworkElement).RequestedTheme;
            this.InitializeComponent();
            PostIds = postIds;
            Discussion = discussion;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private async void SaveAsButton_Click(object sender, RoutedEventArgs e)
        {
            await SaveFile();
        }
        public async Task Download(int min, int range)
        {
            var list = PostIds.GetRange(min, range);
            var posts = await FlarumApiProviders.GetPostsWithId(list, Flarent.Settings.Forum, Flarent.Settings.Token);
            foreach (var post in posts)
                Posts.Add(post);
        }

        public async Task SaveFile()
        {
            var savePicker = new Windows.Storage.Pickers.FileSavePicker();
            savePicker.SuggestedStartLocation =
                Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            savePicker.FileTypeChoices.Add("Markdown文件", new List<string>() { ".md" });

            savePicker.SuggestedFileName = Discussion.Title;
            Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();
            if (file == null)
                return;
            string text = $"# {Discussion.Title}";



            foreach (var post in Posts)
            {
                if ((bool)PosterCheckBox.IsChecked)
                {
                    if(post.User.Id == Discussion.UserId)
                    {
                        if (post.User.Id != 0)
                            text = text.Insert(text.Length, $"\n *** \n **[{post.User.DisplayName}](https://{Flarent.Settings.Forum}/u/{post.User.UserName})**  {post.CreatedAt.Value.ToShortDateString()}  #{post.Number} \n\n");
                        else
                            text = text.Insert(text.Length, $"\n *** \n **已注销** {post.CreatedAt.Value.ToShortDateString()} #{post.Number} \n\n");
                        if (post.ContentHtml != null)
                            text = text.Insert(text.Length, HtmlToMarkdown(post.ContentHtml));
                        else
                            text = text.Insert(text.Length, post.SpecialContent.Description);
                    }
                }
                else
                {
                    if (post.User.Id != 0)
                        text = text.Insert(text.Length, $"\n *** \n **[{post.User.DisplayName}](https://{Flarent.Settings.Forum}/u/{post.User.UserName})**  {post.CreatedAt.Value.ToShortDateString()}  #{post.Number} \n\n");
                    else
                        text = text.Insert(text.Length, $"\n *** \n **已注销** {post.CreatedAt.Value.ToShortDateString()} #{post.Number} \n\n");
                    if (post.ContentHtml != null)
                        text = text.Insert(text.Length, HtmlToMarkdown(post.ContentHtml));
                    else
                        text = text.Insert(text.Length, post.SpecialContent.Description);
                }

            }
            await Windows.Storage.FileIO.WriteTextAsync(file, text);
            Hide();
        }
        public string HtmlToMarkdown(string html)
        {
            var converter = new ReverseMarkdown.Converter();
            var str = converter.Convert(html);
            str = str.Replace("<del>", "~~");//删除线替换
            str = str.Replace("</del>", "~~");//删除线替换
            return str;
        }

        private async void StartDownloadButton_Click(object sender, RoutedEventArgs e)
        {
            StartDownloadButton.Visibility = Visibility.Collapsed;
            DownloadPanel.Visibility = Visibility.Visible;
            for (int i = 0; i < PostIds.Count; i += 30)
            {
                DownloadingTextBlock.Text = $"正在下载({i}/{PostIds.Count})...";
                if (i + 30 > PostIds.Count)
                {
                    await Download(i, PostIds.Count - i);
                    DownloadingProgressBar.Value = 100;
                }
                else
                {
                    await Download(i, 30);
                    var value = (decimal)(i + 30) / PostIds.Count;
                    //double value = ((i + 30) / (PostIds.Count))*100;
                    DownloadingProgressBar.Value = (double)value * 100;
                }
            }
            DownloadingTextBlock.Text = "下载完成";
            ExportPanel.Visibility = Visibility.Visible;
        }
    }
}
