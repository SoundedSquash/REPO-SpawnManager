using System.Collections.Generic;
using BepInEx.Configuration;
using BepInEx.Logging;

namespace SpawnManager
{
    public static class Settings
    {
        public static ConfigEntry<string> DisabledEnemies { get; set; }
        
        public static ManualLogSource Logger { get; private set; }

        internal static void Initialize(ConfigFile config, ManualLogSource logger)
        {
            Logger = logger;
            
            DisabledEnemies = config.Bind(
                "Enemies",
                "DisabledList",
                "",
                "Comma-separated list of enemy names to disable. (e.g. \"Apex Predator,Headman\")");
        }
        
        public static List<string> GetDisabledEnemyNames()
        {
            if (string.IsNullOrEmpty(DisabledEnemies.Value))
                return new List<string>();
            
            return new List<string>(DisabledEnemies.Value.Split(','));
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
    }
}