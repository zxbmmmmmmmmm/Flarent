using FlarentApp.Helpers;
using FlarumApi;
using FlarumApi.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
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

        public NewDiscussionDialog()
        {
            this.InitializeComponent();
            GetTags();
        }

        private  async void GetTags()
        {
            Tags = await FlarumApiProviders.GetTags(Flarent.Settings.Forum, Flarent.Settings.Token);
            TagsListView.ItemsSource = Tags;
            var maintags = Tags.Where(p => !p.IsChild);
            var childtags = Tags.Where(p => p.IsChild);
            
        }

        private void AddTagButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void TagsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listview = ((ListView)sender);
            List<object> items = listview.SelectedItems.ToList();
            var selected = new List<Tag>();
            
        }
    }
}
