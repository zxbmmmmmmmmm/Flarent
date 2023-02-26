using Microsoft.Toolkit.Uwp.Notifications;
using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FlarentApp.Views.Dialogs
{
    public sealed partial class WhatsNewDialog : ContentDialog
    {
        public WhatsNewDialog()
        {
            RequestedTheme = (Window.Current.Content as FrameworkElement).RequestedTheme;
            InitializeComponent();
            var builder = new ToastContentBuilder()
                .AddText("调查问卷", hintMaxLines: 1)
                .AddText("点击填写Flarent调查问卷");
        }
    }
}
