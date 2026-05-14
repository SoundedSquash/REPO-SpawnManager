using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using SpawnManager.Extensions;
using SpawnManager.Managers;

namespace SpawnManager.Patches
{
    [HarmonyPatch(nameof(Arena))]
    public class ArenaPatches
    {
        public static void ClearItems()
        {
            if (SemiFunc.IsNotMasterClient()) return;
            
            List<string> disabledItemNames = Settings.GetDisabledSettingsEntryListNames(Settings.DisabledItems);
            var disabledItemNamesForLevel = Settings.GetDisabledItemsForLevel(LevelManager.GenericArenaLevelName);
            disabledItemNames.AddRange(disabledItemNamesForLevel);
            
            ItemManager.instance.purchasedItems.RemoveItems(disabledItemNames);
        }

        [HarmonyPatch(typeof(Arena), "ArenaInitMultiplayer")]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            var targetMethod = AccessTools.Method(typeof(ItemManager), nameof(ItemManager.GetAllItemVolumesInScene));
            var clearItemsMethod = AccessTools.Method(typeof(ArenaPatches), nameof(ClearItems));

            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Callvirt &&
                    codes[i].operand is MethodInfo method &&
                    method == targetMethod)
                {
                    codes.InsertRange(i, new[]
                    {
                        new CodeInstruction(OpCodes.Call, clearItemsMethod)
                    });

                    return codes;
                }
            }

            throw new Exception("Could not find GetAllItemVolumesInScene");
        }
    }
}