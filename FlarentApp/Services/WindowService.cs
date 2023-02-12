using FlarentApp.Views.WindowPages;
using FlarumApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.WindowManagement;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;

namespace FlarentApp.Services
{
    internal class WindowService
    {
        private static WindowService _current;

        public static WindowService Current => _current ?? (_current = new WindowService());
        public async Task<AppWindow> CreateReplyWindow(Discussion discussion, Post post = null, string text = "", string referer = null,bool release = true)
        {
            var tuple = new Tuple<Discussion, Post, string, string>(discussion, post, text , referer);
            var window = await CreateWindow(typeof(ReplyPage), tuple, $"回复：{discussion.Title}",release);
            return window;
        }
        public async Task<AppWindow> CreateWindow(Type pageType ,object parameter , string title ,bool release = true)
        {
            AppWindow appWindow = await AppWindow.TryCreateAsync();

            var titleBar = appWindow.TitleBar;
            var color = new Color { A = 25, R = 128, G = 128, B = 128 };
            titleBar.ExtendsContentIntoTitleBar = true;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonHoverBackgroundColor = color;

            Frame appWindowContentFrame = new Frame();
            appWindow.Title = title;
            appWindowContentFrame.Navigate(pageType, parameter);
            ReplyPage page = (ReplyPage)appWindowContentFrame.Content;
            page.MyAppWindow = appWindow;
            ElementCompositionPreview.SetAppWindowContent(appWindow, appWindowContentFrame);
            await appWindow.TryShowAsync();
            if(release)
            {
                appWindow.Closed += delegate
                {
                    appWindowContentFrame.Content = null;
                    appWindow = null;
                };
            }
            return appWindow;
        }
    }
}
