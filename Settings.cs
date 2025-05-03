using System.Collections.Generic;
using BepInEx.Configuration;
using BepInEx.Logging;
using SpawnManager.Extensions;
using SpawnManager.Managers;

namespace SpawnManager
{
    public static class Settings
    {
        public static ConfigEntry<bool> ShowSpawnManagerButton { get; private set; } = null!;
        
        public static ConfigEntry<string> DisabledEnemies { get; private set; } = null!;

        public static ConfigEntry<string> DisabledValuables { get; private set; } = null!;

        public static ConfigEntry<string> DisabledLevels { get; private set; } = null!;

        public static IDictionary<string, ConfigEntry<string>> DisabledLevelEnemies { get; private set; } = new Dictionary<string, ConfigEntry<string>>();
        
        public static ConfigEntry<string> DisabledItems { get; private set; } = null!;

        public static IDictionary<string, ConfigEntry<string>> DisabledLevelItems { get; private set; } = new Dictionary<string, ConfigEntry<string>>();
        
        public static ConfigEntry<string> DefaultValuable { get; private set; } = null!;

        public static ManualLogSource Logger { get; private set; } = null!;
        
        private static ConfigFile Config { get; set; } = null!;
        
        private static bool LevelsInitialized { get; set; } = false;
        
        private static bool ItemsInitialized { get; set; } = false;
        
        private const string HideFromRepoConfig = "HideREPOConfig";

        internal static void Initialize(ConfigFile config, ManualLogSource logger)
        {
            Config = config;
            Logger = logger;
            
            var setting = ShowSpawnManagerButton = Config.Bind(
                "General",
                "ShowSpawnManagerButton",
                true,
                "Set to false to hide the Spawn Manager button on the main menu.");
            setting.SettingChanged += MenuModManager.OnShowSpawnManagerButtonChanged;
            
            _ = Config.Bind(
                "Do Not Use",
                "Use Spawn Manager button on main menu",
                0f,
                "Use Spawn Manager button on main menu");
            
            DisabledEnemies = Config.Bind(
                "Enemies",
                "DisabledList",
                "",
                new ConfigDescription("Comma-separated list of enemy names to disable. (e.g. \"Apex Predator,Headman\")", null, HideFromRepoConfig));
            
            DefaultValuable = Config.Bind(
                "Valuables",
                "Default",
                "Valuable Goblet",
                new ConfigDescription("A single tiny valuable used to fill when there aren't enough valuables enabled in the level. (e.g. \"Valuable Diamond\")", null, HideFromRepoConfig));
            
            DisabledValuables = Config.Bind(
                "Valuables",
                "DisabledList",
                "",
                new ConfigDescription("Comma-separated list of valuable names to disable. (e.g. \"Valuable Television,Valuable Diamond Display\")", null, HideFromRepoConfig));
            
            DisabledLevels = Config.Bind(
                "Levels",
                "DisabledList",
                "",
                new ConfigDescription("Comma-separated list of level names to disable. (e.g. \"Level - Manor\")", null, HideFromRepoConfig));
            
            DisabledItems = Config.Bind(
                "Items",
                "DisabledList",
                "",
                new ConfigDescription("Comma-separated list of item names to disable. (e.g. \"Item Cart Medium\")", null, HideFromRepoConfig));
        }

        internal static void InitializeEnemiesLevels()
        {
            // Ensure this is only run once.
            if (LevelsInitialized) return;
            
            foreach (var level in LevelManager.GetAllLevels())
            {
                var configBinding = Config.Bind(
                "Levels", // Should be in Enemies. TODO change to Enemies if values can be transferred.
                $"{level.FriendlyName()} - Disabled Enemies",
                "",
                new ConfigDescription("Comma-separated list of enemy names to disable in this level. (e.g. \"Apex Predator,Headman\")", null, HideFromRepoConfig));

                DisabledLevelEnemies.Add(level.name, configBinding);
            }
            
            LevelsInitialized = true;
        }

        internal static void InitializeItemsLevels()
        {
            // Ensure this is only run once.
            if (ItemsInitialized) return;
            
            foreach (var level in LevelManager.GetAllLevels())
            {
                var configBinding = Config.Bind(
                "Items",
                $"{level.FriendlyName()} - Disabled Items",
                "",
                new ConfigDescription("Comma-separated list of item names to disable in this level. (e.g. \"Item Cart Medium\")", null, HideFromRepoConfig));

                DisabledLevelItems.Add(level.name, configBinding);
            }
            
            ItemsInitialized = true;
        }

        public static ISet<string> GetDisabledEnemiesForLevel(string level)
        {
            if (DisabledLevelEnemies.TryGetValue(level, out ConfigEntry<string> disabledEnemies))
            {
                return new HashSet<string>(ConvertStringToList(disabledEnemies.Value));
            }
            return new HashSet<string>();
        }

        public static ISet<string> GetDisabledItemsForLevel(string level)
        {
            if (DisabledLevelItems.TryGetValue(level, out ConfigEntry<string> disabledItems))
            {
                return new HashSet<string>(ConvertStringToList(disabledItems.Value));
            }
            return new HashSet<string>();
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