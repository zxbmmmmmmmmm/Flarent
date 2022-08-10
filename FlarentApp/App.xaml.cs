using System;

using FlarentApp.Services;
using FlarentApp.Views.Controls;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;

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
        }

        protected override async void OnActivated(IActivatedEventArgs args)
        {
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
