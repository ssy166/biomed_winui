using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using biomed.Services;
using biomed.ViewModels;
using biomed.Views;
using System;

namespace biomed
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; }
        public static Window MainWindow { get; private set; }
        public static XamlRoot MainRoot => MainWindow.Content.XamlRoot;

        public static T GetService<T>() where T : class
        {
            if (Services.GetService(typeof(T)) is not T service)
            {
                throw new ArgumentException($"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.");
            }
            return service;
        }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            Services = ConfigureServices();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            base.OnLaunched(args);
            
            MainWindow = Services.GetRequiredService<MainWindow>();
            MainWindow.Activate();
        }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Services
            services.AddSingleton<ApiClient>();
            services.AddSingleton<IUserStore, UserStore>();
            services.AddSingleton<INavigationService, NavigationService>();

            // ViewModels
            services.AddSingleton<ShellViewModel>();
            services.AddTransient<HomeViewModel>();
            services.AddTransient<ResearchViewModel>();
            services.AddTransient<ResearchPlatformViewModel>();
            services.AddTransient<AccountViewModel>();
            services.AddTransient<EducationViewModel>();
            services.AddTransient<LoginViewModel>();

            // Views/Pages
            services.AddTransient<HomePage>();
            services.AddTransient<ResearchPage>();
            services.AddTransient<ResearchPlatformPage>();
            services.AddTransient<AccountPage>();
            services.AddTransient<AuthenticationContentPage>();
            services.AddTransient<ShellPage>();
            services.AddTransient<VideoPlayerPage>();

            // Main Window
            services.AddSingleton<MainWindow>();

            return services.BuildServiceProvider();
        }
    }
}
