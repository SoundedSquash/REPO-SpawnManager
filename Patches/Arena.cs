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
        public static void ArenaInitMultiplayerPrefix(Arena __instance)
        {
            if (SemiFunc.IsNotMasterClient()) return;
            
            List<string> disabledItemNames = Settings.GetDisabledSettingsEntryListNames(Settings.DisabledItems);
            var disabledItemNamesForLevel = Settings.GetDisabledItemsForLevel(LevelManager.GenericArenaLevelName);
            disabledItemNames.AddRange(disabledItemNamesForLevel);
            
            __instance.itemsMelee.RemoveItems(disabledItemNames);
            __instance.itemsGuns.RemoveItems(disabledItemNames);
            __instance.itemsCarts.RemoveItems(disabledItemNames);
            __instance.itemsDronesAndOrbs.RemoveItems(disabledItemNames);
            __instance.itemsHealth.RemoveItems(disabledItemNames);
            __instance.itemsUsables.RemoveItems(disabledItemNames);
        }
    }
}