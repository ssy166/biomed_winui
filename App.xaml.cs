using Microsoft.UI.Xaml;
using Microsoft.Extensions.DependencyInjection;
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
        private Window m_window;

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
            m_window = new MainWindow();

            var rootFrame = m_window.Content as Microsoft.UI.Xaml.Controls.Frame;
            if (rootFrame == null)
            {
                rootFrame = new Microsoft.UI.Xaml.Controls.Frame();
                m_window.Content = rootFrame;
            }

            rootFrame.Navigate(typeof(ShellPage));

            m_window.Activate();
        }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Core Services (Singletons)
            services.AddSingleton<ApiClient>();
            services.AddSingleton<IUserStore, UserStore>();
            services.AddSingleton<NavigationService>();

            // ViewModels (Transient)
            services.AddTransient<ShellViewModel>();
            services.AddTransient<LoginViewModel>();
            services.AddTransient<HomeViewModel>();
            services.AddTransient<ResearchViewModel>();

            // Views / Pages (Transient)
            services.AddTransient<ShellPage>();
            services.AddTransient<LoginPage>();
            services.AddTransient<HomePage>();
            services.AddTransient<FormulaPage>();
            services.AddTransient<ResearchPage>();
            
            // Register MainWindow to be accessible from DI
            services.AddSingleton<MainWindow>();

            return services.BuildServiceProvider();
        }
    }
}
