using FlarentApp.Helpers;
using FlarentApp.Services;
using FlarentApp.Views.DetailPages;
using FlarumApi;
using FlarumApi.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace FlarentApp.Views
{
    public sealed partial class UsersPage : Page, INotifyPropertyChanged
    {
        public UsersPage()
        {
            InitializeComponent();
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<User> Users = new ObservableCollection<User>();
        public string LinkNext
        {
            get => _linkNext;
            set
            {
                if (_linkNext != value)
                {
                    _linkNext = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LinkNext)));
                }
            }
        }
        private string _linkNext;
        public string SortBy = "-lastSeenAt";
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            GetUsers();

        }

        private async void GetUsers()
        {
            var link = $"https://{Flarent.Settings.Forum}/api/users?sort={SortBy}";
            var data = await FlarumApiProviders.GetUsers(link, Flarent.Settings.Token);
            Users = data.Item1;
            LinkNext = data.Item2;
            UsersListView.ItemsSource = Users;
        }

        private void NavigationView_ItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
        {
            var item = (Microsoft.UI.Xaml.Controls.NavigationViewItem)sender.SelectedItem;
            SortBy = item.Tag.ToString();
            GetUsers();
        }

        private async void LoadMoreButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            LoadMoreButton.IsEnabled = false;
            var data = await FlarumApiProviders.GetUsers (LinkNext, Flarent.Settings.Token);
            LinkNext = data.Item2;
            foreach (var user in data.Item1)
                Users.Add(user);
            LoadMoreButton.IsEnabled = true;
        }

        private void UsersListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var clicked = e.ClickedItem as User;
            if (Flarent.Settings.ViewUsersInPane)
                NavigationService.OpenInRightPane(typeof(UserDetailPage), clicked.Id);
            else
                NavigationService.Navigate<UserDetailPage>(clicked.Id);
        }
    }
}
