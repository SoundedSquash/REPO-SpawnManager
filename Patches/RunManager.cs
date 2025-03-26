using HarmonyLib;
using SpawnManager.Managers;

namespace SpawnManager.Patches
{
    [HarmonyPatch(typeof(RunManager))]
    public static class RunManagerPatches
    {
        [HarmonyPatch("Awake")]
        [HarmonyPrefix]
        [HarmonyPriority(Priority.Last)]
        static void RunManagerStartPrefix()
        {
            if (SemiFunc.MenuLevel())
            {
                LevelManager.RestoreLevels();
                ValuableManager.RestoreValuableObjects();
                return;
            }

            Settings.Logger.LogDebug("Removing valuables.");
            ValuableManager.RemoveValuables();
        }
        [HarmonyPatch(nameof(RunManager.SetRunLevel))]
        [HarmonyPrefix]
        static void RunManagerChangeLevelPrefix(RunManager __instance, ref Level ___previousRunLevel)
        {
            LevelManager.RemoveLevels();

            if (__instance.levels.Count == 1)
            {
                // Clear the previous level so it can pick the same one.
                ___previousRunLevel = null;
            }
        }
    }
}