// klasa statyczna do ustawiania aktualnego stylu

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using projekt.Config;
using projekt.Interfaces;

namespace projekt.Styling
{
    public static class AppStyle
    {
        private static IAppStyle _current;

        public static IAppStyle Current => _current ??= GetStyle(SettingsManager.Settings.Theme);

        // Event wywoływany przy zmianie stylu, używany w klasach komponentów
        public static event EventHandler StyleChanged;

        public static void UpdateStyle(string theme)
        {
            _current = GetStyle(theme);
            StyleChanged?.Invoke(null, EventArgs.Empty);
        }

        private static IAppStyle GetStyle(string theme)
        {
            return theme?.ToLower() switch
            {
                "dark" => new DarkStyle(),
                "light" => new LightStyle(),
                _ => new LightStyle()
            };
        }

        public static void SetStyle()
        {
            string newTheme = AppStyle.Current is DarkStyle ? "light" : "dark";
            AppStyle.UpdateStyle(newTheme);
        }
    }

}
