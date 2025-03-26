using System.Collections.Generic;
using BepInEx.Configuration;
using BepInEx.Logging;

namespace SpawnManager
{
    public static class Settings
    {
        public static ConfigEntry<string> DisabledEnemies { get; set; }
        
        public static ConfigEntry<string> DisabledValuables { get; set; }
        
        public static ConfigEntry<string> DisabledLevels { get; set; }
        
        public static ManualLogSource Logger { get; private set; }

        internal static void Initialize(ConfigFile config, ManualLogSource logger)
        {
            Logger = logger;
            
            DisabledEnemies = config.Bind(
                "Enemies",
                "DisabledList",
                "",
                "Comma-separated list of enemy names to disable. (e.g. \"Apex Predator,Headman\")");
            
            DisabledValuables = config.Bind(
                "Valuables",
                "DisabledList",
                "",
                "Comma-separated list of valuable names to disable. (e.g. \"Valuable Television,Valuable Diamond Display\")");
            
            DisabledLevels = config.Bind(
                "Levels",
                "DisabledList",
                "",
                "Comma-separated list of level names to disable. (e.g. \"Level - Manor\")");
        }
        
        public static List<string> GetDisabledSettingsEntryListNames(ConfigEntry<string> settingsVariable)
        {
            return ConvertStringToList(settingsVariable.Value);
        }
        
        private static List<string> ConvertStringToList(string str)
        {
            if (string.IsNullOrEmpty(str))
                return new List<string>();
            return new List<string>(str.Split(',', System.StringSplitOptions.RemoveEmptyEntries));
        }

        public static void UpdateSettingsListEntry(ConfigEntry<string> settingsVariable, string entry, bool enabled)
        {
            var currentList = GetDisabledSettingsEntryListNames(settingsVariable);
            
            if (enabled)
            {
                currentList.Remove(entry);
            }
            else
            {
                if (!currentList.Contains(entry))
                    currentList.Add(entry);
            }
            
            settingsVariable.Value = string.Join(",", currentList);
        }

        public static bool IsSettingsListEntryEnabled(ConfigEntry<string> settingsVariable, string entry)
        {
            var disabledEntries = GetDisabledSettingsEntryListNames(settingsVariable);
            return !disabledEntries.Contains(entry);
        }
    }
}