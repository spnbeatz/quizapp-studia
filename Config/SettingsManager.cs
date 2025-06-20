// klasa do zarządzania ustawieniami użytkownika

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace projekt.Config
{
    public static class SettingsManager
    {
        private static readonly string FilePath = "appsettings.json";
        public static AppSettings Settings { get; private set; } = new AppSettings();

        public static void Load()
        {
            if (File.Exists(FilePath))
            {
                string json = File.ReadAllText(FilePath);
                Settings = JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
            }
        }

        public static void Save()
        {
            string json = JsonSerializer.Serialize(Settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FilePath, json);
        }
    }
}
