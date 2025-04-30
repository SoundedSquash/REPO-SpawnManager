using HarmonyLib;
using SpawnManager.Managers;

namespace SpawnManager.Patches
{
    [HarmonyPatch(typeof(RunManager))]
    public static class RunManagerPatches
    {
        // We need this for multiplayer to work when hosting.
        [HarmonyPatch("Awake")]
        [HarmonyPrefix]
        [HarmonyPriority(Priority.Last)]
        static void RunManagerAwakePrefix()
        {
            if (SemiFunc.MenuLevel())
            {
                return;
            }

            Settings.Logger.LogDebug("Removing valuables.");
            ValuableManager.RemoveValuables();
        }
        
        [HarmonyPatch(nameof(RunManager.SetRunLevel))]
        [HarmonyPrefix]
        static void RunManagerChangeLevelPrefix(RunManager __instance, ref Level ___previousRunLevel)
        {
            Settings.Logger.LogDebug("Removing levels.");
            LevelManager.RemoveLevels();
            Settings.Logger.LogDebug("Removing items.");
            ItemsManager.RemoveItems();

            if (__instance.levels.Count == 1)
            {
                // Clear the previous level so it can pick the same one.
                ___previousRunLevel = null!;
            }
        }
    }
}