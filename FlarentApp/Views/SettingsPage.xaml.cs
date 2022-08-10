using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using FlarentApp.Helpers;
using FlarentApp.Services;
using FlarentApp.Views.Controls;
using FlarentApp.Views.Dialogs;
using FlarumApi;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace FlarentApp.Views
{
    // TODO: Add other settings as necessary. For help see https://github.com/microsoft/TemplateStudio/blob/main/docs/UWP/pages/settings-codebehind.md
    // TODO: Change the URL for your privacy policy in the Resource File, currently set to https://YourPrivacyUrlGoesHere
    public sealed partial class SettingsPage : Page, INotifyPropertyChanged
    {
        private ElementTheme _elementTheme = ThemeSelectorService.Theme;

        public ElementTheme ElementTheme
        {
            get { return _elementTheme; }

            set { Set(ref _elementTheme, value); }
        }

        private string _versionDescription;

        public string VersionDescription
        {
            get { return _versionDescription; }

            set { Set(ref _versionDescription, value); }
        }

        public SettingsPage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            VersionDescription = GetVersionDescription();
            await Task.CompletedTask;
        }

        private string GetVersionDescription()
        {
            var appName = "AppDisplayName".GetLocalized();
            var package = Package.Current;
            var packageId = package.Id;
            var version = packageId.Version;

            return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }

        private async void ThemeChanged_CheckedAsync(object sender, RoutedEventArgs e)
        {
            var param = (sender as RadioButton)?.CommandParameter;

            if (param != null)
            {
                await ThemeSelectorService.SetThemeAsync((ElementTheme)param);
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


        private void OpenPaneButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.OpenInRightPane(typeof(Page));
        }

        private async void ViewUpdateButton_Click(object sender, RoutedEventArgs e)
        {
            await new WhatsNewDialog().ShowAsync();
        }

        private void AcrylicToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            var shell = Window.Current.Content as ShellPage;//获取当前正在显示的页面
            if (AcrylicToggleSwitch.IsOn == true)
            {
                var color = App.Current.Resources["AcrylicBackgroundFillColorDefaultBrush"] as AcrylicBrush;
                shell.navigationView.Background = color;
                shell.rightFrame.Background = color;

            }
            else
            {
                var color = App.Current.Resources["NavigationViewDefaultPaneBackground"] as Brush;
                shell.navigationView.Background = color;
                var split = App.Current.Resources["ApplicationPageBackgroundThemeBrush"] as Brush;
                shell.rightFrame.Background = split;

            }
        }

        private async void UpdateForumInfoBtn_Click(object sender, RoutedEventArgs e)
        {
            UpdateForumInfoBtn.IsEnabled = false;
            try
            {
                await UpdateForumInfo();
            }
            finally
            {
                UpdateForumInfoBtn.IsEnabled = true;
            }
        }

        private async void ChangeForumBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            sender.IsEnabled = false;
            try
            {
                Flarent.Settings.Forum = sender.Text;
                var shell = Window.Current.Content as ShellPage;//获取当前正在显示的页面
                shell.Logout();//退出登录
                await UpdateForumInfo();
                new Toast("切换成功",TimeSpan.FromSeconds(2)).Show();
            }
            finally
            {
                sender.IsEnabled = true;
            }

        }
        public async Task UpdateForumInfo()
        {
            var link = $"https://{Flarent.Settings.Forum}/api";
            var forum = await FlarumApiProviders.GetForumInfo(link, Flarent.Settings.Token);
            Flarent.Settings.ForumInfo = forum;
        }


        private void ChangeForumBox_LostFocus(object sender, RoutedEventArgs e)
        {
            ChangeForumBox.Text = Flarent.Settings.Forum;
        }

    }
}
