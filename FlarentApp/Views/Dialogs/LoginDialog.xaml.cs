using FlarentApp.Helpers;
using FlarentApp.ViewModels.Dialogs;
using FlarumApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
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
    public sealed partial class LoginDialog : ContentDialog
    {
        public LoginDialogVM ViewModel = new();
        public LoginDialog()
        {
            ViewModel.Dialog = this;
            RequestedTheme = (Window.Current.Content as FrameworkElement).RequestedTheme;
            this.InitializeComponent();
        }

        private void UserNameTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter) MyPasswordBox.Focus(FocusState.Keyboard);

        }

        private void MyPasswordBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter) ViewModel.LoginCommand.Execute(null);
        }

    }
}
