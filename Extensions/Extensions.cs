using System.Collections.Generic;

namespace SpawnManager.Extensions
{
    public static class Extensions
    {
        // ValuableObject
        public static string FriendlyName(this ValuableObject valuableObject)
        {
            return valuableObject.name.Replace("Valuable ", "");
        }
        
        // Level
        public static string GetKey(this Level level)
        {
            return level.name;
        }
        public static string FriendlyName(this Level level)
        {
            return level.name.Replace("Level - ", "");
        }
        
        // LevelValuables
        public static string GetKey(this LevelValuables levelValuables)
        {
            return levelValuables.name;
        }
        
        // Item
        public static string ToItemFriendlyName(this string item)
        {
            return item.Replace("Item ", "").Trim();
        }

        public static bool IsGenericList(this LevelValuables levelValuables)
        {
            return levelValuables.name.Contains("Generic");
        }
        
        public static void RemoveItems(this List<Item> items, List<string> disabledItemNames)
        {

            if (items.Count == 0)
            {
                Settings.Logger.LogDebug($"[RemoveItems] {nameof(items)} is null or empty - skipping removal.");
                return;
            }

            if (disabledItemNames.Count == 0)
            {
                Settings.Logger.LogDebug($"[RemoveItems] No disabled items to check - skipping removal.");
                return;
            }

            var itemsToRemove = new List<Item>();
            foreach (var item in items)
            {
                var friendlyName = item.name?.ToItemFriendlyName() ?? "";
                if (disabledItemNames.Contains(friendlyName))
                {
                    itemsToRemove.Add(item);
                }
            }

            foreach (var item in itemsToRemove)
            {
                Settings.Logger.LogDebug($"[RemoveItems] Removed item {item.itemName?.ToItemFriendlyName() ?? "unknown"} from {nameof(items)}.");
                items.Remove(item);
            }
        }
    }
}