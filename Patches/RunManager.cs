using HarmonyLib;
using SpawnManager.Managers;

namespace SpawnManager.Patches
{
    [HarmonyPatch(typeof(RunManager))]
    public static class RunManagerPatches
    {
        [HarmonyPatch(nameof(RunManager.SetRunLevel))]
        [HarmonyPrefix]
        static void RunManagerChangeLevelPrefix(RunManager __instance, ref Level ___previousRunLevel)
        {
            Settings.Logger.LogDebug("Removing levels.");
            LevelManager.RemoveLevels();
            Settings.Logger.LogDebug("Removing valuables.");
            ValuableManager.RemoveValuables();

            if (__instance.levels.Count == 1)
            {
                // Clear the previous level so it can pick the same one.
                ___previousRunLevel = null!;
            }
        }
    }
}