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

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace FlarentApp.Views.Controls
{
    public sealed partial class Toast: UserControl
    {

        private string content;
        private TimeSpan showTime;
        private Popup popup;
        private Toast()
        {
            this.InitializeComponent();
            RequestedTheme = (Window.Current.Content as FrameworkElement).RequestedTheme;
            this.popup = new Popup();
            this.Width = Window.Current.Bounds.Width;
            this.Height = Window.Current.Bounds.Height;
            popup.Child = this;
            this.Loaded += Notification_Loaded;
            this.Unloaded += Notification_Unloaded;
        }
        public Toast(string content, TimeSpan showTime) : this()
        {
            this.content = content;
            this.showTime = showTime;
        }
        public Toast(string content) : this(content, TimeSpan.FromSeconds(1))
        {

        }
        public void Show()
        {
            this.popup.IsOpen = true;
        }
        private void Notification_Unloaded(object sender, RoutedEventArgs e)
        {
            Window.Current.SizeChanged -= Current_SizeChanged;
        }

        private void Notification_Loaded(object sender, RoutedEventArgs e)
        {
            NotificationContent.Text = this.content;
            this.Notification.BeginTime = this.showTime;
            this.Notification.Begin();
            this.Notification.Completed += Notification_Completed;
            Window.Current.SizeChanged += Current_SizeChanged;
        }

        private void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            this.Width = e.Size.Width;
            this.Height = e.Size.Height;
        }

        private void Notification_Completed(object sender, object e)
        {
            this.popup.IsOpen = false;
        }

    }
}
