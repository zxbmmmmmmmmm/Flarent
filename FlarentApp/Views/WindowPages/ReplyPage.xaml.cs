using ColorCode.Compilation.Languages;
using FlarentApp.Helpers;
using FlarentApp.Helpers.Converters;
using FlarentApp.Services;
using FlarentApp.Views.Controls;
using FlarentApp.Views.DetailPages;
using FlarumApi;
using FlarumApi.Helpers;
using FlarumApi.Models;
using Markdig;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.WindowManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace FlarentApp.Views.WindowPages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ReplyPage : Page
    {
        public Discussion Discussion;
        public Post Post;
        public string Text;
        public string Referer;
        public bool Success = false;
        public AppWindow MyAppWindow { get; set; }

        public ReplyPage()
        {
            this.InitializeComponent();

        }
        private void EditZone_TextChanged(object sender, RoutedEventArgs e)
        {
            LoadingProgressBar.Visibility = Visibility.Collapsed;
            string value = string.Empty;
            EditZone.Document.GetText(Windows.UI.Text.TextGetOptions.AdjustCrlf, out value);
            if (value == string.Empty) PreviewTextBlock.Html = " ";
            else PreviewTextBlock.Html = Markdig.Markdown.ToHtml(value);
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var tuple = e.Parameter as Tuple<Discussion,Post,string,string>;
            Discussion = tuple.Item1;
            Post = tuple.Item2;
            Referer = tuple.Item4;
            if (tuple.Item3 != "")
                EditZone.Document.Selection.TypeText(tuple.Item3);
        }

        private async void ReplyButton_Click(object sender, RoutedEventArgs e)
        {
            LoadingProgressBar.Visibility = Visibility.Visible;
            LoadingProgressBar.ShowError = false;
            string text = string.Empty;
            EditZone.Document.GetText(Windows.UI.Text.TextGetOptions.UseCrlf, out text);
            ReplyButton.IsEnabled = false;
            try
            {
                if (Post == null)//发帖
                {
                    var data = await FlarumApiProviders.ReplyAsync(text, $"https://{Flarent.Settings.Forum}/api/posts", (int)Discussion.Id, Flarent.Settings.Token);
                    var reply = data.Item1;
                    if (data.Item2 == "")
                    {
                        await MyAppWindow.CloseAsync();
                        var postId = (int)reply["data"]["id"];
                        var shell = Window.Current.Content as ShellPage;//获取当前正在显示的页面
                        var frame = shell.shellFrame;
                        if (frame.Content is DiscussionDetailPage page)
                        {
                            await page.GetDiscussion();
                            page.TurnToLastPage();
                        }
                        NavigationService.OpenInRightPane(typeof(PostDetailPage), postId);
                    }
                    else
                    {
                        LoadingProgressBar.ShowError = true;
                        ReplyButton.IsEnabled = true;
                    }
                }
                else
                {
                    await Edit(text);
                }
            }

            catch
            {
                LoadingProgressBar.ShowError = true;
                ReplyButton.IsEnabled = true;
            }

        }
        public async Task Edit(string text)
        {
            var data = await FlarumApiProviders.EditAsync(text, $"https://{Flarent.Settings.Forum}/api/posts/{(int)Post.Id}", (int)Post.Id, Flarent.Settings.Token, Referer);
            var reply = data.Item1;
            if (data.Item2 == "")
            {
                Text = data.Item2;
                new Toast("编辑成功").Show();
                var discussion = Post.Discussion;
                Post = data.Item1;
                Post.Discussion = discussion;
                var postId = data.Item1.Id;
                Success = true;
                await MyAppWindow.CloseAsync();
            }
            else
            {
                LoadingProgressBar.ShowError = true;
                ReplyButton.IsEnabled = true;
            }
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(e.NewSize.Width>= 1040)
            {
                Grid.SetRow(PreviewGrid, 1);
                Grid.SetColumn(PreviewGrid, 1); SecColumn.Width = new GridLength(1,GridUnitType.Star);
                PreviewGrid.Margin = new Thickness(16, 0, 16, 0);
            }
            else
            {
                Grid.SetRow(PreviewGrid, 3);
                Grid.SetColumn(PreviewGrid, 0);
                SecColumn.Width = GridLength.Auto;
                PreviewGrid.Margin = new Thickness(0);
            }

        }

        private async void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            StorageFile file = await picker.PickSingleFileAsync();//选择文件
            if (file == null)
                return;
            string boundary = string.Format("------WebKitFormBoundary{0}", DateTime.Now.Ticks.ToString("x"));

            var content = new MultipartFormDataContent(boundary);
            var streamData = await file.OpenReadAsync();
            var bytes = new byte[streamData.Size];
            using (var dataReader = new DataReader(streamData))
            {
                await dataReader.LoadAsync((uint)streamData.Size);
                dataReader.ReadBytes(bytes);
            }
            var streamContent = new ByteArrayContent(bytes);
            string strBoundary =  DateTime.Now.Ticks.ToString("x");
            content.Headers.ContentType = new MediaTypeHeaderValue($"multipart/form-data;boundary={boundary}") ;
            content.Add(streamContent, "files[]");
            await FlarumApiProviders.UploadAsync($"https://{Flarent.Settings.Forum}/api/fof/upload",Flarent.Settings.Token,content);
        }

    }
}
