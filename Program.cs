using Microsoft.Extensions.DependencyInjection;
using projekt.Services;
using projekt.Data;
using projekt.Forms;
using projekt.Interfaces;
using projekt.Routing;
using projekt.Utils;
using projekt.Config;
using projekt.Styling;

namespace projekt
{
    internal static class Program
    {

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Wczytanie ustawieñ aplikacji
            SettingsManager.Load();

            ApplicationConfiguration.Initialize();

            var services = new ServiceCollection();

            // pobranie routów na podstawie folderu Screens
            services.AddScreensAsRoutes(out var discoveredRoutes, null);

            services.AddSingleton<IDatabase, Database>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IQuizService, QuizService>();
            services.AddTransient<ISearchService, SearchService>();
            services.AddSingleton<App>();
            services.AddSingleton(provider =>
            {
                var router = new Router();
                router.SetDiscoveredRoutes(discoveredRoutes);
                router.LogRoutes();
                return router;
            });

            var provider = services.BuildServiceProvider();

            var app = provider.GetRequiredService<App>();

            AppStyle.UpdateStyle(SettingsManager.Settings.Theme);

            Application.Run(app);
        }
    }
}