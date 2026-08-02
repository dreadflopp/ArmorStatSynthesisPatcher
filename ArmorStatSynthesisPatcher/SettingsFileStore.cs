using Newtonsoft.Json;
using System.IO;

namespace ArmourStatsSynthesisPatcher
{
    public static class SettingsFileStore
    {
        public const string SettingsFileName = "settings.json";

        public static Settings Load(string? settingsFolder)
        {
            var path = GetSettingsPath(settingsFolder);
            if (!File.Exists(path))
            {
                return new Settings();
            }

            try
            {
                var json = File.ReadAllText(path);
                var settings = JsonConvert.DeserializeObject<Settings>(json);
                return settings ?? new Settings();
            }
            catch
            {
                // Fall back to defaults if the file cannot be parsed.
                return new Settings();
            }
        }

        public static void Save(string? settingsFolder, Settings settings)
        {
            var path = GetSettingsPath(settingsFolder);
            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(path, json);
        }

        private static string GetSettingsPath(string? settingsFolder)
        {
            if (string.IsNullOrWhiteSpace(settingsFolder))
            {
                return Path.Combine(Environment.CurrentDirectory, SettingsFileName);
            }

            return Path.Combine(settingsFolder, SettingsFileName);
        }
    }
}
