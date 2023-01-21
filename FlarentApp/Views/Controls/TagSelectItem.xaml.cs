using FlarentApp.Views.Dialogs;
using FlarumApi.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace FlarentApp.Views.Controls
{
    public sealed partial class TagSelectItem : UserControl , IDisposable ,INotifyPropertyChanged
    {
        public Tag SelectedTag;
        private bool disposedValue;

        public Tag FlarumTag
        {
            get { return (Tag)GetValue(FlarumTagProperty); }
            set {
                SetValue(FlarumTagProperty, value);
            }
        }
        public static readonly DependencyProperty FlarumTagProperty =
           DependencyProperty.Register("FlarumTag", typeof(Tag), typeof(TagSelectItem), new PropertyMetadata(new Tag()));

        public event PropertyChangedEventHandler PropertyChanged;

        public TagSelectItem()
        {
            this.InitializeComponent();
            this.DataContextChanged += (s, e) => Bindings.Update();
            NewDiscussionDialog.Current.TagsListView.ItemClick += TagsListView_ItemClick;
        }


        public void TagsListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Bindings.Update();
            var clicked = e.ClickedItem as Tag;
            if (clicked != FlarumTag)//仅在取消选中时清空
                return;
            for (int i = 0; i < ChildrenTagsListView.Items.Count; i++)
            {
                
                var container = ChildrenTagsListView.ContainerFromIndex(i) as ListViewItem;
                if (container == null)
                    continue;
                container.Visibility = Visibility.Visible;
                var tag = ChildrenTagsListView.Items[i] as Tag;
                tag.IsSelected = false;
            }
            
        }
        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                    NewDiscussionDialog.Current.TagsListView.ItemClick -= TagsListView_ItemClick;

                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                FlarumTag = null;
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~TagSelectItem()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private void ChildrenTagsListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var tag = e.ClickedItem as Tag;
            NewDiscussionDialog.Current.SelectedTags.Add(tag);
            tag.IsSelected = !tag.IsSelected;
            var item = ((ItemsControl)sender).ContainerFromItem(e.ClickedItem) as ListViewItem;
            var select = item.ContentTemplateRoot as TagSelectItem;
            select.Bindings.Update();
            foreach (object child in ChildrenTagsListView.Items)
            {
                var childTag = child as Tag;
                var container = ChildrenTagsListView.ContainerFromItem(child) as ListViewItem;
                if(tag.IsSelected&& container != item)
                    container.Visibility= Visibility.Collapsed;
                else
                    container.Visibility = Visibility.Visible;
            }
            NewDiscussionDialog.Current.ResetSelectedTags();

        }
    }
}
