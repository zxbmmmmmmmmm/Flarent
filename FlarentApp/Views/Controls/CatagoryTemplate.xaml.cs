using FlarentApp.Services;
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
    public sealed partial class CatagoryTemplate : UserControl
    {
        public CatagoryTemplate()
        {
            this.InitializeComponent();
            this.DataContextChanged += (s, e) => Bindings.Update();

        }
        public Tag TagData
        {
            get { return (Tag)GetValue(CatagoryProperty); }
            set { SetValue(CatagoryProperty, value); }
        }
        public static readonly DependencyProperty CatagoryProperty =
           DependencyProperty.Register("TagData", typeof(Tag), typeof(CatagoryTemplate), new PropertyMetadata(new Tag()));

        private void ChildrenTagsListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var clicked = e.ClickedItem as Tag;
            NavigationService.Navigate<HomePage>(clicked);
        }
    }
}
