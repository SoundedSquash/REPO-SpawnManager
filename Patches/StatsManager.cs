using HarmonyLib;
using SpawnManager.Managers;

namespace SpawnManager.Patches
{
    [HarmonyPatch(typeof(StatsManager))]
    public static class StatsManagerPatches
    {
        [HarmonyPatch("RunStartStats")]
        [HarmonyPostfix]
        [HarmonyPriority(Priority.Last)]
        static void StatsManagerRunStartStatsPostfix()
        {
            Settings.Logger.LogDebug("Removing items.");
            ItemsManager.RemoveItems();
        }
    }
}