using FlarentApp.Services;
using FlarentApp.Views.DetailPages;
using FlarumApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    public sealed partial class SmallTagTemplate : UserControl
    {
        public SmallTagTemplate()
        {
            this.InitializeComponent();
            this.DataContextChanged += (s, e) => Bindings.Update();
            TagButton.Click -= TagButton_Click;
            TagButton.Click += TagButton_Click;
        }


        public string TagName
        {
            get { return (string)GetValue(TagNameProperty); }
            set { SetValue(TagNameProperty, value); }
        }

        public string Icon
        {
            get { return (string)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }
        public Tag TagData
        {
            get { return (Tag)GetValue(TagDataProperty); }
            set { SetValue(TagDataProperty, value); }
        }
        public static readonly DependencyProperty TagNameProperty =
           DependencyProperty.Register("TagName", typeof(string), typeof(TagTemplate), new PropertyMetadata(""));

        public static readonly DependencyProperty IconProperty =
           DependencyProperty.Register("Icon", typeof(string), typeof(TagTemplate), new PropertyMetadata("fas fa-tag"));

        public static readonly DependencyProperty TagDataProperty =
            DependencyProperty.Register("Icon", typeof(Tag), typeof(TagTemplate), new PropertyMetadata(new Tag { }));

        private void TagButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            var clicked = (Tag)btn.DataContext;
            //e.OriginalSource;
            NavigationService.Navigate<HomePage>(clicked);
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            TagButton.Click -= TagButton_Click;
        }
    }
}
