using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using FlarentApp.Helpers;
using FlarentApp.Services;
using FlarentApp.Views.DetailPages;
using FlarentApp.Views.Dialogs;
using FlarumApi;
using Microsoft.UI.Xaml.Media;
using Newtonsoft.Json;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using WinUI = Microsoft.UI.Xaml.Controls;

namespace FlarentApp.Views
{
    public sealed partial class ShellPage : Page, INotifyPropertyChanged
    {
        private readonly KeyboardAccelerator _altLeftKeyboardAccelerator = BuildKeyboardAccelerator(VirtualKey.Left, VirtualKeyModifiers.Menu);
        private readonly KeyboardAccelerator _backKeyboardAccelerator = BuildKeyboardAccelerator(VirtualKey.GoBack);

        private bool _isBackEnabled;
        private WinUI.NavigationViewItem _selected;

        public bool IsBackEnabled
        {
            get { return _isBackEnabled; }
            set { Set(ref _isBackEnabled, value); }
        }

        public WinUI.NavigationViewItem Selected
        {
            get { return _selected; }
            set { Set(ref _selected, value); }
        }
        public FlarumApi.Models.User User
        {
            get => _user;
            set
            {
                if (_user != value)
                {
                    _user = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(User)));
                }
            }
        }
        private FlarumApi.Models.User _user;
        public ShellPage()
        {
            InitializeComponent();
            DataContext = this;
            Initialize();
            NavigationService.Initialize(splitView, rightFrame);
        }

        private void Initialize()
        {
            NavigationService.Frame = shellFrame;
            NavigationService.NavigationFailed += Frame_NavigationFailed;
            NavigationService.Navigated += Frame_Navigated;
            navigationView.BackRequested += OnBackRequested;
            SetTitleBar();

            if (Flarent.Settings.IsAcrylicEnabled)
            {
                var color = App.Current.Resources["AcrylicBackgroundFillColorDefaultBrush"] as Windows.UI.Xaml.Media.AcrylicBrush;
                navigationView.Background = color;
                rightFrame.Background = color;
            }
            else
            {
                var color = App.Current.Resources["NavigationViewDefaultPaneBackground"] as Brush;
                navigationView.Background = color;
                var split = App.Current.Resources["ApplicationPageBackgroundThemeBrush"] as Brush;
                rightFrame.Background = color;
            }

        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Keyboard accelerators are added here to avoid showing 'Alt + left' tooltip on the page.
            // More info on tracking issue https://github.com/Microsoft/microsoft-ui-xaml/issues/8
            KeyboardAccelerators.Add(_altLeftKeyboardAccelerator);
            KeyboardAccelerators.Add(_backKeyboardAccelerator);
            await Task.CompletedTask;

            if (Flarent.Settings.Token != "")//已登录
                UpdateUserInfo();//更新用户数据
            else
                User = Default.NotLoginedUser;//未登录用户

        }
        public void SetTitleBar()
        {
            var coreTitleBar = Windows.ApplicationModel.Core.CoreApplication.GetCurrentView().TitleBar;
            var appTitleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
            var color = new Color { A =25,R=128,G=128,B=128};
            coreTitleBar.ExtendViewIntoTitleBar = true;
            appTitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            appTitleBar.ButtonBackgroundColor = Colors.Transparent;
            appTitleBar.ButtonHoverBackgroundColor = color;

            Window.Current.SetTitleBar(AppTitleBar);

        }
        /// <summary>
        /// 更新用户数据
        /// </summary>
        private async void UpdateUserInfo()
        {
            var link = $"https://{Flarent.Settings.Forum}/api/users/{Flarent.Settings.UserId.ToString()}";
            var user = await FlarumApiProviders.GetUser(link, Flarent.Settings.Token);
            Flarent.Settings.UserInfo = JsonConvert.SerializeObject(user);
            User = user;
            GetNotifications();
        }

        private void Frame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw e.Exception;
        }

        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {
            IsBackEnabled = NavigationService.CanGoBack;
            if (e.SourcePageType == typeof(SettingsPage))
            {
                Selected = navigationView.SettingsItem as WinUI.NavigationViewItem;
                return;
            }

            var selectedItem = GetSelectedItem(navigationView.MenuItems, e.SourcePageType);
            if (selectedItem != null)
            {
                Selected = selectedItem;
            }
        }

        private WinUI.NavigationViewItem GetSelectedItem(IEnumerable<object> menuItems, Type pageType)
        {
            foreach (var item in menuItems.OfType<WinUI.NavigationViewItem>())
            {
                if (IsMenuItemForPageType(item, pageType))
                {
                    return item;
                }

                var selectedChild = GetSelectedItem(item.MenuItems, pageType);
                if (selectedChild != null)
                {
                    return selectedChild;
                }
            }

            return null;
        }

        private bool IsMenuItemForPageType(WinUI.NavigationViewItem menuItem, Type sourcePageType)
        {
            var pageType = menuItem.GetValue(NavHelper.NavigateToProperty) as Type;
            return pageType == sourcePageType;
        }

        private async void OnItemInvoked(WinUI.NavigationView sender, WinUI.NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                NavigationService.Navigate(typeof(SettingsPage), null, args.RecommendedNavigationTransitionInfo);
            }
            else
            {
                var selectedItem = args.InvokedItemContainer as WinUI.NavigationViewItem;
                var pageType = selectedItem?.GetValue(NavHelper.NavigateToProperty) as Type;

                if (pageType != null)
                {
                    NavigationService.Navigate(pageType, null, args.RecommendedNavigationTransitionInfo);
                }
                else
                {
                    if(selectedItem.Tag.ToString()== "User")
                    {
                        if(Flarent.Settings.Token != "")
                        {
                            var flyout = FlyoutBase.GetAttachedFlyout(UserMenuItem);
                            flyout.ShowAt(UserMenuItem);
                            
                        }
                        else
                        {
                            await new LoginDialog().ShowAsync();
                            if (Flarent.Settings.Token != "")//已经登录成功
                                UpdateUserInfo();
                        }

                    }
                    else if(selectedItem.Tag.ToString() == "Notification")
                    {
                        var flyout = FlyoutBase.GetAttachedFlyout(NotificationMenuItem);
                        flyout.ShowAt(NotificationMenuItem);
                    }
                }
            }
        }

        private void OnBackRequested(WinUI.NavigationView sender, WinUI.NavigationViewBackRequestedEventArgs args)
        {
            NavigationService.GoBack();
        }

        private static KeyboardAccelerator BuildKeyboardAccelerator(VirtualKey key, VirtualKeyModifiers? modifiers = null)
        {
            var keyboardAccelerator = new KeyboardAccelerator() { Key = key };
            if (modifiers.HasValue)
            {
                keyboardAccelerator.Modifiers = modifiers.Value;
            }

            keyboardAccelerator.Invoked += OnKeyboardAcceleratorInvoked;
            return keyboardAccelerator;
        }

        private static void OnKeyboardAcceleratorInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            var result = NavigationService.GoBack();
            args.Handled = result;
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

        private void UserActonListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var clicked = ((StackPanel)e.ClickedItem).Tag.ToString();
            if (clicked == "PersonalPage")
            {
                NavigationService.Navigate<UserDetailPage>(Flarent.Settings.UserId);
            }
            else if(clicked == "MyFavorite")
            {
                NavigationService.Navigate<HomePage>("&filter[subscription]=following");
            }
            else if(clicked == "Logout")
            {
                Logout();
            }
            var flyout = FlyoutBase.GetAttachedFlyout(UserMenuItem);
            flyout.Hide();
        }
        public async void GetNotifications()
        {
            var notifications = await FlarumApiProviders.GetNotifications(Flarent.Settings.Forum,Flarent.Settings.Token);
            if (notifications == null)//登录已过期
                Logout();
            else
            {
                var unReaded = notifications.Where(p => p.IsRead == false);
                if(unReaded.Count() == 0)
                    NotificationsInfoBadge.Visibility = Visibility.Collapsed;
                else
                {
                    NotificationsInfoBadge.Visibility = Visibility.Visible;
                    NotificationsInfoBadge.Value = unReaded.Count();
                }

            }
        }
        /// <summary>
        /// 退出登录
        /// </summary>
        public void Logout()
        {
            Flarent.Settings.UserId = 0;
            Flarent.Settings.Token = "";
            var user = Default.NotLoginedUser;
            //var user = new FlarumApi.Models.User { DisplayName = "未登录" };
            Flarent.Settings.UserInfo = JsonConvert.SerializeObject(user);
            User = user;
            NotificationsInfoBadge.Visibility = Visibility.Collapsed;
        }

        private void navigationView_DisplayModeChanged(WinUI.NavigationView sender, WinUI.NavigationViewDisplayModeChangedEventArgs args)
        {
            if(args.DisplayMode == WinUI.NavigationViewDisplayMode.Minimal)
            {
                TitieBarContent.Margin = new Thickness(25, 0, 0, 0);
            }
        }

        private void navigationView_PaneClosing(WinUI.NavigationView sender, WinUI.NavigationViewPaneClosingEventArgs args)
        {
            TitieBarContent.Margin = new Thickness(25, 0, 0, 0);
            AppTitle.MaxWidth = 256;
            PageNameTextBlock.Visibility = Visibility.Visible;
        }

        private void navigationView_PaneOpening(WinUI.NavigationView sender, object args)
        {
            if (sender.DisplayMode == WinUI.NavigationViewDisplayMode.Minimal)
            {
                TitieBarContent.Margin = new Thickness(25, 0, 0, 0);
                PageNameTextBlock.Visibility = Visibility.Collapsed;
            }
            else
            {
                TitieBarContent.Margin = new Thickness(0);
            }
            AppTitle.MaxWidth = LayoutHelper.Values.TitleTextBlockMaxWidth;

        }
    }
}
