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
        }
    }
}
