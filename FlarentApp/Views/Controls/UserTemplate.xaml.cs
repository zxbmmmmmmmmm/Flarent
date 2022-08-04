using FlarentApp.Helpers.Converters;
using FlarentApp.Helpers;
using FlarumApi;
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
    public sealed partial class UserTemplate : UserControl, INotifyPropertyChanged
    {
        public UserTemplate()
        {
            this.InitializeComponent();
            this.DataContextChanged += (s, e) => Bindings.Update();
            var converter = new DateConverter();


        }
        public event PropertyChangedEventHandler PropertyChanged;

        public string ShowText
        {
            get { return (string)GetValue(ShowTextProperty); }
            set { SetValue(ShowTextProperty, value); }
        }

        public static readonly DependencyProperty ShowTextProperty =
            DependencyProperty.Register("ShowText", typeof(string), typeof(UserTemplate), new PropertyMetadata(""));
        public User UserData
        {
            get { return (User)GetValue(UserDataProperty); }
            set { SetValue(UserDataProperty, value); }
        }

        public static readonly DependencyProperty UserDataProperty =
            DependencyProperty.Register("UserData", typeof(User), typeof(UserTemplate), new PropertyMetadata(Preset.DefaultUser));
    }
}
