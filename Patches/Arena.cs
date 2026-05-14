using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using SpawnManager.Extensions;
using SpawnManager.Managers;

namespace SpawnManager.Patches
{
    [HarmonyPatch(nameof(Arena))]
    public class ArenaPatches
    {
        [HarmonyPatch(typeof(Arena), "ArenaInitMultiplayer")]
        [HarmonyPrefix]
        public static void ArenaInitMultiplayerPrefix()
        {
            Settings.Logger.LogDebug($"Patching arena init for multiplayer.");
            if (SemiFunc.IsNotMasterClient()) return;
            
            List<string> disabledItemNames = Settings.GetDisabledSettingsEntryListNames(Settings.DisabledItems);
            var disabledItemNamesForLevel = Settings.GetDisabledItemsForLevel(LevelManager.GenericArenaLevelName);
            disabledItemNames.AddRange(disabledItemNamesForLevel);

            var arena = Arena.instance;
            
            RemoveItems(arena.itemsMelee, disabledItemNames);
            RemoveItems(arena.itemsGuns, disabledItemNames);
            RemoveItems(arena.itemsCarts, disabledItemNames);
            RemoveItems(arena.itemsDronesAndOrbs, disabledItemNames);
            RemoveItems(arena.itemsHealth, disabledItemNames);
            RemoveItems(arena.itemsUsables, disabledItemNames);
        }

        private static void RemoveItems(List<Item> items, List<string> disabledItemNames)
        {
            items.Where(item => disabledItemNames.Contains(item.itemName.ToItemFriendlyName())).ToList().ForEach(item =>
            {
                Settings.Logger.LogDebug($"Removed item {item.itemName.ToItemFriendlyName()} from {nameof(items)}.");
                items.Remove(item);
            });
        }
    }
}