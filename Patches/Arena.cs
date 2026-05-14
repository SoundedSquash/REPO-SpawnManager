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
            var itemsMid = Traverse.Create(Arena.instance).Field("itemsMid").GetValue<List<Item>>();
            // Remove items from mid-game list and full list.
            itemsMid.RemoveItems(disabledItemNames);
            ItemManager.instance.purchasedItems.RemoveItems(disabledItemNames);
        }

        [HarmonyPatch(typeof(Arena), "ArenaInitMultiplayer")]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> ArenaInitMultiplayerTranspiler(IEnumerable<CodeInstruction> instructions)
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

        // TODO Prevent error loop for SpawnMidWeapons when list is empty
        // [HarmonyPatch(typeof(Arena), "SpawnMidWeapons")]
        // [HarmonyTranspiler]
        // static IEnumerable<CodeInstruction> SpawnMidWeaponsTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        // {
        //     var codes = new List<CodeInstruction>(instructions);
        //
        //     var itemsMidField = AccessTools.Field(typeof(Arena), "itemsMid");
        //     var midSpawnerTimerField = AccessTools.Field(typeof(Arena), "midSpawnerTimer");
        //     var listCountGetter = AccessTools.PropertyGetter(typeof(List<Item>), "Count");
        //
        //     int insertIndex = -1;
        //
        //     for (int i = 0; i < codes.Count - 4; i++)
        //     {
        //         if (codes[i].opcode == OpCodes.Ldarg_0 &&
        //             codes[i + 1].opcode == OpCodes.Ldfld && Equals(codes[i + 1].operand, itemsMidField) &&
        //             codes[i + 2].opcode == OpCodes.Ldc_I4_0 &&
        //             codes[i + 3].opcode == OpCodes.Ldarg_0 &&
        //             codes[i + 4].opcode == OpCodes.Ldfld && Equals(codes[i + 4].operand, itemsMidField))
        //         {
        //             insertIndex = i;
        //             break;
        //         }
        //     }
        //
        //     if (insertIndex < 0)
        //         throw new Exception("Could not find itemsMid random selection block");
        //
        //     var continueLabel = il.DefineLabel();
        //     codes[insertIndex].labels.Add(continueLabel);
        //
        //     var injected = new List<CodeInstruction>
        //     {
        //         new CodeInstruction(OpCodes.Ldarg_0),
        //         new CodeInstruction(OpCodes.Ldfld, itemsMidField),
        //         new CodeInstruction(OpCodes.Callvirt, listCountGetter),
        //         new CodeInstruction(OpCodes.Brtrue_S, continueLabel),
        //
        //         new CodeInstruction(OpCodes.Ldarg_0),
        //         new CodeInstruction(OpCodes.Ldc_R4, 0f),
        //         new CodeInstruction(OpCodes.Stfld, midSpawnerTimerField),
        //         new CodeInstruction(OpCodes.Ret)
        //     };
        //
        //     codes.InsertRange(insertIndex, injected);
        //     return codes;
        // }
    }
}