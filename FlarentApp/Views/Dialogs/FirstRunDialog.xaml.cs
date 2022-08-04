using FlarentApp.Helpers;
using FlarumApi;
using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FlarentApp.Views.Dialogs
{
    public sealed partial class FirstRunDialog : ContentDialog
    {
        public FirstRunDialog()
        {
            RequestedTheme = (Window.Current.Content as FrameworkElement).RequestedTheme;
            InitializeComponent();
            UpdateForum();
        }
        public async void UpdateForum()
        {
            Flarent.Settings.ForumInfo = await FlarumApiProviders.GetForumInfo($"https://{Flarent.Settings.Forum}/api", null);
        }
    }
}
