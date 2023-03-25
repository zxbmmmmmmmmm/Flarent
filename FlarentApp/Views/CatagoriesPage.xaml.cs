using FlarentApp.Helpers;
using FlarentApp.Services;
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
    public sealed partial class CatagoriesPage : Page, INotifyPropertyChanged
    {
        public ObservableCollection<Tag> Catagories = new ObservableCollection<Tag>();
        public CatagoriesPage()
        {
            InitializeComponent();
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
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            GetCatagories();
        }
        public async void GetCatagories()
        {
            Catagories = await FlarumApiProviders.GetTags(Flarent.Settings.Forum, Flarent.Settings.Token,true);
            CatagoriesListView.ItemsSource = Catagories;
            LoadingProgressRing.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void CatagoriesListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var clicked = e.ClickedItem as Tag;
            NavigationService.Navigate<HomePage>(clicked);
        }
    }
}
