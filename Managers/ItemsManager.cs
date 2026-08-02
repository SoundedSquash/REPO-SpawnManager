using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using SpawnManager.Extensions;

namespace SpawnManager.Managers
{
    // Called Items as the game already has an ItemManager. 
    public static class ItemsManager
    {
        private static Dictionary<string, Item> _removedList = new Dictionary<string, Item>();

        private static bool StatsManagerItemDictionaryIsAvailable => StatsManager.instance != null && StatsManager.instance.itemDictionary != null;

        public static Dictionary<string, Item> GetAllItems()
        {
            IEnumerable<KeyValuePair<string, Item>> items = StatsManagerItemDictionaryIsAvailable
                ? StatsManager.instance.itemDictionary
                : Enumerable.Empty<KeyValuePair<string, Item>>();

            return items
                .Concat(_removedList)
                .Concat(GetRepoLibItems().Select(item => new KeyValuePair<string, Item>(item.name, item)))
                .GroupBy(pair => pair.Key)
                .ToDictionary(group => group.Key, group => group.First().Value);
        }

        public static void RemoveItems()
        {
            // Restore all items so different levels can disable only their items.
            RestoreItems();
            Traverse.Create(StatsManager.instance).Method("LoadItemsFromFolder").GetValue();
            
            if (SemiFunc.IsNotMasterClient()) return;
            if (SemiFunc.RunIsArena()) return;
            
            List<string> disabledItemNames = Settings.GetDisabledSettingsEntryListNames(Settings.DisabledItems);
            
            string? currentLevelName = RunManager.instance.levelCurrent?.name;

            if (currentLevelName != null)
            {
                // Overrides for generic shop/arena to cover variants
                if (SemiFunc.RunIsShop()) currentLevelName = LevelManager.GenericShopLevelName;
                
                var disabledItemNamesForLevel = Settings.GetDisabledItemsForLevel(currentLevelName);
                disabledItemNames.AddRange(disabledItemNamesForLevel);
            }
            
            if (!StatsManagerItemDictionaryIsAvailable) return;
            if (StatsManager.instance.itemDictionary.Count == 0) return;

            StatsManager.instance.item.Where(keyValuePair => disabledItemNames.Contains(keyValuePair.Key.ToItemFriendlyName())).ToList().ForEach(keyValuePair =>
            {
                StatsManager.instance.item.Remove(keyValuePair.Key);
            });
            
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
        
        private static IEnumerable<Item> GetRepoLibItems()
        {
            if (!PluginManager.IsPluginInstalled(Constants.RepoLibGuid)) return Enumerable.Empty<Item>();

            var itemsTraverse = CreateRepoLibItemTraverse();
            
            if (itemsTraverse == null) return Enumerable.Empty<Item>();
            
            // This private field never seems to be cleared and always has all REPOLib's items.
            return itemsTraverse
                .Field("_itemsToRegister")
                .GetValue<IEnumerable<Item>>() ?? Enumerable.Empty<Item>();
        }

        private static Traverse? CreateRepoLibItemTraverse()
        {
            var repoAssembly = AppDomain.CurrentDomain
                .GetAssemblies()
                .FirstOrDefault(assembly => assembly.GetName().Name == "REPOLib");

            var itemsType = repoAssembly?.GetType("REPOLib.Modules.Items");
            
            return itemsType == null ? null : Traverse.Create(itemsType);
        }
    }
}