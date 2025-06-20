// klasa do automatycznego rozpoznawania Screenów i dodawania ich do ServiceCollection

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;

namespace projekt.Routing
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddScreensAsRoutes(this IServiceCollection services, out Dictionary<string, Type> discoveredRoutes, Assembly assembly = null)
        {
            assembly ??= Assembly.GetExecutingAssembly();
            discoveredRoutes = new();

            var screenTypes = assembly.GetTypes()
                .Where(t =>
                    typeof(UserControl).IsAssignableFrom(t) &&
                    t.Namespace?.Contains("Screens") == true &&
                    !t.IsAbstract && !t.IsInterface
                );

            foreach (var type in screenTypes)
            {
                var route = GetRouteFromClassName(type.Name);
                discoveredRoutes[route] = type;
                services.AddTransient(type);

                Console.WriteLine($"[Router] Zarejestrowano screen: {type.FullName} jako ruta: '{route}'");
            }
            return services;
        }

        private static string GetRouteFromClassName(string className)
        {
            return className
                .Replace("Screen", "")
                .Replace("View", "")
                .ToLower();
        }
    }
}

