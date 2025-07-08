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
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            var window = GetService<MainWindow>();
            window.Activate();
        }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Services
            services.AddSingleton<ApiClient>();
            services.AddSingleton<IUserStore, UserStore>();
            services.AddSingleton<INavigationService, NavigationService>();

            // ViewModels
            services.AddTransient<HomeViewModel>();
            services.AddTransient<ResearchViewModel>();
            services.AddTransient<AccountViewModel>();

            // Views/Pages
            services.AddTransient<HomePage>();
            services.AddTransient<ResearchPage>();
            services.AddTransient<AccountPage>();
            services.AddTransient<AuthenticationContentPage>();

            // Main Window
            services.AddSingleton<MainWindow>();

            return services.BuildServiceProvider();
        }
    }
}
