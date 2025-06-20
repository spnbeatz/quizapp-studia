using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using projekt.Routing;
using projekt.Interfaces;

namespace projekt.Routing
{
    public class Router
    {
        private readonly Dictionary<string, Type> _routes = new();
        private readonly Dictionary<string, UserControl> _cache = new();
        private readonly Stack<(string route, object param)> _history = new();
        private readonly Stack<(string route, object param)> _forwardStack = new();

        private Panel _hostPanel;
        private IServiceProvider _services;

        public event Action<string> CurrentRouteChanged;
        public string CurrentRoute { get; private set; }

        public void Initialize(Panel hostPanel,  IServiceProvider services)
        {
            _hostPanel = hostPanel;
            _services = services;
            
        }

        public void Navigate(string route, object parameter = null)
        {
            if (!_routes.TryGetValue(route.ToLower(), out var viewType)) return;

            _history.Push((route, parameter));
            _forwardStack.Clear();

            LoadView(viewType, route, parameter);
        }

        public void Back()
        {
            if (_history.Count <= 1) return;

            var current = _history.Pop();
            _forwardStack.Push(current);

            var (route, param) = _history.Peek();
            NavigateWithoutStack(route, param);
        }

        private void NavigateWithoutStack(string route, object parameter = null)
        {
            if (_routes.TryGetValue(route.ToLower(), out var viewType))
            {
                LoadView(viewType, route, parameter);
            }
        }

        public void Forward()
        {
            if (_forwardStack.Count == 0) return;

            var (route, param) = _forwardStack.Pop();
            _history.Push((route, param));
            NavigateWithoutStack(route, param);
        }

        private void LoadView(Type viewType, string route, object parameter)
        {
            UserControl control;

            if (_cache.TryGetValue(route, out var cachedControl))
            {
                control = cachedControl;
            }
            else
            {
                control = (UserControl)_services.GetRequiredService(viewType);
                _cache[route] = control;
            }

            if (control is IRoutable routable)
                routable.OnNavigatedTo(parameter);

            _hostPanel.Controls.Clear();
            _hostPanel.Controls.Add(control);
            control.Dock = DockStyle.Fill;

            CurrentRoute = route;
            CurrentRouteChanged?.Invoke(route);
        }

        public void SetDiscoveredRoutes(Dictionary<string, Type> routes)
        {
            foreach (var kvp in routes)
                _routes[kvp.Key] = kvp.Value;
        }

        public void LogRoutes(string filePath = "routes-log.txt")
        {
            var log = new StringBuilder();
            log.AppendLine($"[ROUTER ROUTES LOG] {DateTime.Now}");

            foreach (var kvp in _routes)
            {
                log.AppendLine($"Route: '{kvp.Key}' => Type: {kvp.Value.FullName}");
            }

            File.WriteAllText(filePath, log.ToString());
        }


    }
}
