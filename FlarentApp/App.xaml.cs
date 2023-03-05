using System;

using FlarentApp.Services;
using FlarentApp.Views.Controls;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.ApplicationModel.Activation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using FlarentApp.Views.DetailPages;

namespace FlarentApp
{
    public sealed partial class App : Application
    {
        private Lazy<ActivationService> _activationService;
        private ElementTheme _elementTheme = ThemeSelectorService.Theme;

        private ActivationService ActivationService
        {
            get { return _activationService.Value; }
        }

        public App()
        {
            InitializeComponent();
            if (_elementTheme == ElementTheme.Light)
                RequestedTheme = ApplicationTheme.Light;
            else if(_elementTheme == ElementTheme.Dark)
                RequestedTheme = ApplicationTheme.Dark;
            UnhandledException += OnAppUnhandledException;

            // Deferred execution until used. Check https://docs.microsoft.com/dotnet/api/system.lazy-1 for further info on Lazy<T> class.
            _activationService = new Lazy<ActivationService>(CreateActivationService);
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            if (!args.PrelaunchActivated)
            {
                await ActivationService.ActivateAsync(args);
            }
            AppCenter.Start("d251ad1e-bfb3-4893-a926-971831e68d03",
                    typeof(Analytics), typeof(Crashes));
        }

        protected override async void OnActivated(IActivatedEventArgs args)
        {
            if (args is ToastNotificationActivatedEventArgs toastActivationArgs)
            {
                ToastArguments toastArgs = ToastArguments.Parse(toastActivationArgs.Argument);

                if (toastArgs["discussion"] != null)
                {
                    await ActivationService.ActivateAsync(args);
                    NavigationService.Navigate(typeof(DiscussionDetailPage), int.Parse(toastArgs["discussion"]));
                    Analytics.TrackEvent("NotificationClicked");
                    return;
                }
                else
                {
                    await Launcher.LaunchUriAsync(new Uri("https://wj.qq.com/s2/11777368/65f5"));
                    return;
                }
            }
            await ActivationService.ActivateAsync(args);
        }

        private void OnAppUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            new Toast(e.Message,TimeSpan.FromSeconds(2)).Show();
            // TODO: Please log and handle the exception as appropriate to your scenario
            // For more info see https://docs.microsoft.com/uwp/api/windows.ui.xaml.application.unhandledexception
        }

        private ActivationService CreateActivationService()
        {
            return new ActivationService(this, typeof(Views.HomePage), new Lazy<UIElement>(CreateShell));
        }

        private UIElement CreateShell()
        {
            return new Views.ShellPage();
        }
    }
}
