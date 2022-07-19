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
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“内容对话框”项模板

namespace FlarentApp.Views.Dialogs
{
    public sealed partial class ShellContentDialog : ContentDialog
    {
        public ShellContentDialog(Type pageType, object parameter = null, NavigationTransitionInfo infoOverride = null,string title = null)
        {
            this.InitializeComponent();
            shellFrame.Navigate(pageType, parameter, infoOverride);
            shellFrame.Width = Window.Current.Bounds.Width * 0.8;
            shellFrame.Height = Window.Current.Bounds.Height * 0.8;
            if (title != null)
                TitleTextBlock.Text = title;
        }
        public static ShellContentDialog Create(Type pageType, object parameter = null, NavigationTransitionInfo infoOverride = null, string title = null)
        {
            return new ShellContentDialog(pageType, parameter, infoOverride,title);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }
    }
}
