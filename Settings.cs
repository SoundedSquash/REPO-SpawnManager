using System.Collections.Generic;
using BepInEx.Configuration;
using BepInEx.Logging;

namespace SpawnManager
{
    public static class Settings
    {
        public static ConfigEntry<string> DisabledEnemies { get; set; }
        
        public static ConfigEntry<string> DisabledValuables { get; set; }
        
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
        }
        
        public static List<string> GetDisabledEnemyNames()
        {
            return ConvertStringToList(DisabledEnemies.Value);
        }
        
        private static List<string> ConvertStringToList(string str)
        {
            if (string.IsNullOrEmpty(str))
                return new List<string>();
            return new List<string>(str.Split(',', System.StringSplitOptions.RemoveEmptyEntries));
        }

        public static void UpdateEnemyEntry(string enemyName, bool enabled)
        {
            var currentList = GetDisabledEnemyNames();
            
            if (enabled)
            {
                currentList.Remove(enemyName);
            }
            else
            {
                if (!currentList.Contains(enemyName))
                    currentList.Add(enemyName);
            }
            
            SaveEnemyList(currentList);
        }

        public static void SaveEnemyList(List<string> enemyNames)
        {
            DisabledEnemies.Value = string.Join(",", enemyNames);
        }

        public static bool IsEnemyEnabled(string enemyName)
        {
            var disabledEnemies = GetDisabledEnemyNames();
            return !disabledEnemies.Contains(enemyName);
        }
        
        public static List<string> GetDisabledValuableNames()
        {
            return ConvertStringToList(DisabledValuables.Value);
        }
    }
}