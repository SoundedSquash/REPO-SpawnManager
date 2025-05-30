﻿using System.Collections.Generic;
using System.Linq;
using SpawnManager.Extensions;

namespace SpawnManager.Managers
{
    // Called Items as the game already has an ItemManager. 
    public static class ItemsManager
    {
        private static Dictionary<string, Item> _removedList = new Dictionary<string, Item>();

        private static bool StatsManagerItemDictionaryIsAvailable => StatsManager.instance != null && StatsManager.instance.itemDictionary != null;

        public static Dictionary<string, Item> GetAllItems() => _removedList.Concat(StatsManager.instance.itemDictionary)
            .ToDictionary(pair => pair.Key, pair => pair.Value);

        public static void RemoveItems()
        {
            // Restore all items so different levels can disable only their items.
            RestoreItems();
            
            if (SemiFunc.IsNotMasterClient()) return;
            if (!StatsManagerItemDictionaryIsAvailable) return;
            if (StatsManager.instance.itemDictionary.Count == 0) return;
            
            List<string> disabledItemNames = Settings.GetDisabledSettingsEntryListNames(Settings.DisabledItems);
            
            string? currentLevelName = RunManager.instance.levelCurrent?.name;

            if (currentLevelName != null)
            {
                var disabledItemNamesForLevel = Settings.GetDisabledItemsForLevel(currentLevelName);
                disabledItemNames.AddRange(disabledItemNamesForLevel);
            }

            StatsManager.instance.itemDictionary.Where(keyValuePair => disabledItemNames.Contains(keyValuePair.Key.ToItemFriendlyName())).ToList().ForEach(keyValuePair =>
            {
                Settings.Logger.LogDebug($"Removed item {keyValuePair.Key.ToItemFriendlyName()}.");
                _removedList.TryAdd(keyValuePair.Key, keyValuePair.Value);
                StatsManager.instance.itemDictionary.Remove(keyValuePair.Key);
            });
        }

        public static void RestoreItems()
        {
            if (!StatsManagerItemDictionaryIsAvailable) return;
            if (_removedList.Count == 0) return;

            for (var i = _removedList.Count - 1; i >= 0; i--)
            {
                var keyValuePair = _removedList.ElementAt(i);
                Settings.Logger.LogDebug($"Restored item {keyValuePair.Key.ToItemFriendlyName()}.");
                StatsManager.instance.itemDictionary.TryAdd(keyValuePair.Key, keyValuePair.Value);
                _removedList.Remove(keyValuePair.Key);
            }
        }
    }
}