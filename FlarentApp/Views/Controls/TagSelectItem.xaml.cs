using FlarumApi.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public sealed partial class TagSelectItem : UserControl
    {
        public Tag SelectedTag;
        public Tag FlarumTag
        {
            get { return (Tag)GetValue(FlarumTagProperty); }
            set { SetValue(FlarumTagProperty, value); }
        }
        public static readonly DependencyProperty FlarumTagProperty =
           DependencyProperty.Register("FlarumTag", typeof(Tag), typeof(TagSelectItem), new PropertyMetadata(new Tag()));
        public TagSelectItem()
        {
            this.InitializeComponent();
            this.DataContextChanged += (s, e) => Bindings.Update();
        }
    }
}
