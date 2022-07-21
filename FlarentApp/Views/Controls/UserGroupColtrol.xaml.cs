using FlarumApi.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public sealed partial class UserGroupControl : UserControl
    {
        public UserGroupControl()
        {
            this.InitializeComponent();
            this.DataContextChanged += (s, e) => Bindings.Update();
        }
        public ObservableCollection<UserGroup> UserGroups
        {
            get { return (ObservableCollection<UserGroup>)GetValue(UserGroupsProperty); }
            set { SetValue(UserGroupsProperty, value); }
        }

        public static readonly DependencyProperty UserGroupsProperty =
            DependencyProperty.Register("UserGroups", typeof(ObservableCollection<UserGroup>), typeof(UserTemplate), new PropertyMetadata(new ObservableCollection<UserGroup>()));
    }
}
