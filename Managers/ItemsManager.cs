using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

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
            if (SemiFunc.IsNotMasterClient()) return;
            if (!StatsManagerItemDictionaryIsAvailable) return;
            
            List<string> disabledItemNames = Settings.GetDisabledSettingsEntryListNames(Settings.DisabledItems);

            StatsManager.instance.itemDictionary.Where(keyValuePair => disabledItemNames.Contains(keyValuePair.Key)).ToList().ForEach(keyValuePair =>
            {
                Settings.Logger.LogDebug($"Removed item {keyValuePair.Value.itemName}.");
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
                Settings.Logger.LogDebug($"Restored item {keyValuePair.Value.itemName}.");
                StatsManager.instance.itemDictionary.TryAdd(keyValuePair.Key, keyValuePair.Value);
                _removedList.Remove(keyValuePair.Key);
            }
        }
    }
}